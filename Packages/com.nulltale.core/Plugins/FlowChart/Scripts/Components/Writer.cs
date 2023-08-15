// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using UnityEngine;
 using System.Collections;
using System.Collections.Generic;
using System.Linq;
 using System.Text;
using System.Globalization;
 using CoreLib;
 using TMPro;

namespace Fungus
{
    /// <summary>
    /// Current state of the writing process.
    /// </summary>
    public enum WriterState
    {
        /// <summary> Invalid state. </summary>
        Invalid,
        /// <summary> Writer has started writing. </summary>
        Start,
        /// <summary> Writing has been paused. </summary>
        Pause,
        /// <summary> Writing has resumed after a pause. </summary>
        Resume,
        /// <summary> Writing has ended. </summary>
        End,
    }

    /// <summary>
    /// Writes text using a typewriter effect to a UI text object.
    /// </summary>
    public class Writer : MonoBehaviour, IDialogInputListener
    {
        [Tooltip("Gameobject containing a Text, Inout Field or Text Mesh object to write to")]
        [SerializeField] protected TMP_Text text;

        [Tooltip("Gameobject to punch when the punch tags are displayed. If none is set, the main camera will shake instead.")]
        [SerializeField] protected GameObject punchObject;

        [Tooltip("Writing characters per second")]
        [SerializeField] protected float writingSpeed = 60;

        [Tooltip("Pause duration for punctuation characters")]
        [SerializeField] protected float punctuationPause = 0.25f;

        [Tooltip("Color of text that has not been revealed yet")]
        [SerializeField] protected Color hiddenTextColor = new Color(1,1,1,0);

        [Tooltip("Write one word at a time rather one character at a time")]
        [SerializeField] protected bool writeWholeWords = false;

        [Tooltip("Force the target text object to use Rich Text mode so text color and alpha appears correctly")]
        [SerializeField] protected bool forceRichText = true;

        [Tooltip("Click while text is writing to finish writing immediately")]
        [SerializeField] protected bool instantComplete = true;

        [SerializeField] protected bool doReadAheadText = true;

        // This property is true when the writer is waiting for user input to continue
        protected bool isWaitingForInput;

        // This property is true when the writer is writing text or waiting (i.e. still processing tokens)
        protected bool isWriting;

        protected float currentWritingSpeed;
        protected float currentPunctuationPause;

        protected bool boldActive = false;
        protected bool italicActive = false;
        protected bool colorActive = false;
        protected string colorText = "";
        protected bool linkActive = false;
        protected string linkText = string.Empty;
        protected bool sizeActive = false;
        protected float sizeValue = 16f;
        protected bool inputFlag;
        protected bool exitFlag;

        //holds number of Word tokens in the currently running Write
        public int WordTokensFound { get; protected set; }

        /// <summary>
        /// Updated during writing of Word tokens, when processed tips over found, fires NotifyAllWordsWritten
        /// </summary>
        public virtual int WordTokensProcessed
        {
            get => wordTokensProcessed;
            protected set
            {
                if(wordTokensProcessed < WordTokensFound && value >= WordTokensFound)
                    NotifyAllWordsWritten();
                
                wordTokensProcessed = value;
            }
        }
        //holds count of number of Word tokens completed
        protected int wordTokensProcessed;

        /// <summary>
        /// Does the currently processing list of Tokens have Word Tokens that are not yet processed
        /// </summary>
        public bool HasWordsRemaining { get { return WordTokensProcessed < WordTokensFound; } }

        public List<IWriterListener> writerListeners = new List<IWriterListener>();

        protected StringBuilder openString = new StringBuilder(256);
        protected StringBuilder closeString = new StringBuilder(256);
        protected StringBuilder leftString = new StringBuilder(1024);
        protected StringBuilder rightString = new StringBuilder(1024);
        protected StringBuilder outputString = new StringBuilder(1024);
        protected StringBuilder readAheadString = new StringBuilder(1024);

        protected string hiddenColorOpen = "";
        protected string hiddenColorClose = "";

        protected int visibleCharacterCount = 0;
        protected int readAheadStartIndex = 0;
        public WriterAudio AttachedWriterAudio { get; set; }

        // =======================================================================
        protected virtual void Awake()
        {
            // Cache the list of child writer listeners
            writerListeners.AddRange(GetComponentsInChildren<IWriterListener>());
            CacheHiddenColorStrings();
        }

        protected virtual void CacheHiddenColorStrings()
        {
            // Cache the hidden color string
            Color32 c = hiddenTextColor;
            hiddenColorOpen = string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>", c.r, c.g, c.b, c.a);
            hiddenColorClose = "</color>";
        }

        protected virtual void Start()
        {
            if (forceRichText)
                text.richText = true;
        }
        
        protected virtual void UpdateOpenMarkup()
        {
            openString.Length = 0;
            
            if (text.richText)
            {
                if (sizeActive)
                {
                    openString.Append("<size=");
                    openString.Append(sizeValue);
                    openString.Append(">"); 
                }
                if (colorActive)
                {
                    openString.Append("<color=");
                    openString.Append(colorText);
                    openString.Append(">");
                }
                if (linkActive)
                {
                    openString.Append("<link=");
                    openString.Append(linkText);
                    openString.Append(">");
                }
                if (boldActive)
                {
                    openString.Append("<b>"); 
                }
                if (italicActive)
                {
                    openString.Append("<i>"); 
                }           
            }
        }
        
        protected virtual void UpdateCloseMarkup()
        {
            closeString.Length = 0;
            
            if (text.richText)
            {
                if (italicActive)
                {
                    closeString.Append("</i>"); 
                }           
                if (boldActive)
                {
                    closeString.Append("</b>"); 
                }
                if (colorActive)
                {
                    closeString.Append("</color>");
                }
                if (linkActive)
                {
                    closeString.Append("</link>");
                }
                if (sizeActive)
                {
                    closeString.Append("</size>"); 
                }
            }
        }

        protected virtual bool CheckParamCount(List<string> paramList, int count) 
        {
            if (paramList == null)
            {
                Debug.LogError("paramList is null");
                return false;
            }
            if (paramList.Count != count)
            {
                Debug.LogError("There must be exactly " + paramList.Count + " parameters.");
                return false;
            }
            return true;
        }

        protected virtual bool TryGetSingleParam(List<string> paramList, int index, float defaultValue, out float value) 
        {
            value = defaultValue;
            if (paramList.Count > index)
            {
                float.TryParse(paramList[index], NumberStyles.Any, CultureInfo.InvariantCulture, out value);
                return true;
            }
            return false;
        }

        protected virtual IEnumerator ProcessTokens(List<TextTagToken> tokens, bool stopAudio, System.Action onComplete)
        {
            // Reset control members
            boldActive = false;
            italicActive = false;
            colorActive = false;
            sizeActive = false;
            WordTokensFound = tokens.Count(x => x.type == TokenType.Words);
            WordTokensProcessed = 0;
            colorText = "";
            sizeValue = 16f;
            currentPunctuationPause = punctuationPause;
            currentWritingSpeed = writingSpeed;

            exitFlag = false;
            isWriting = true;

            var previousTokenType = TokenType.Invalid;

            for (var i = 0; i < tokens.Count; ++i)
            {
                // Pause between tokens if Paused is set
                while (Paused)
                {
                    yield return null;
                }

                var token = tokens[i];

                // Notify listeners about new token
                WriterSignals.DoTextTagToken(this, token, i, tokens.Count);
                
                // Update the read ahead string buffer. This contains the text for any 
                // Word tags which are further ahead in the list. 
                if (doReadAheadText)
                {
                    readAheadString.Length = 0;
                    for (var j = i + 1; j < tokens.Count; ++j)
                    {
                        var readAheadToken = tokens[j];

                        if (readAheadToken.type == TokenType.Words &&
                            readAheadToken.paramList.Count == 1)
                        {
                            readAheadString.Append(readAheadToken.paramList[0]);
                        }
                        else if (readAheadToken.type == TokenType.WaitForInputAndClear)
                        {
                            break;
                        }
                    }
                }

                switch (token.type)
                {
                case TokenType.Words:
                    yield return StartCoroutine(DoWords(token.paramList, previousTokenType));
                    WordTokensProcessed++;
                    break;
                    
                case TokenType.BoldStart:
                    boldActive = true;
                    break;
                    
                case TokenType.BoldEnd:
                    boldActive = false;
                    break;
                    
                case TokenType.ItalicStart:
                    italicActive = true;
                    break;
                    
                case TokenType.ItalicEnd:
                    italicActive = false;
                    break;
                    
                case TokenType.ColorStart:
                    if (CheckParamCount(token.paramList, 1)) 
                    {
                        colorActive = true;
                        colorText = token.paramList[0];
                    }
                    break;
                    
                case TokenType.ColorEnd:
                    colorActive = false;
                    break;

                case TokenType.LinkStart:
                    if (CheckParamCount(token.paramList, 1))
                    {
                        linkActive = true;
                        linkText = token.paramList[0];
                    }
                    break;

                case TokenType.LinkEnd:
                    linkActive = false;
                    break;

                case TokenType.SizeStart:
                    if (TryGetSingleParam(token.paramList, 0, 16f, out sizeValue))
                    {
                        sizeActive = true;
                    }
                    break;

                case TokenType.SizeEnd:
                    sizeActive = false;
                    break;

                case TokenType.Wait:
                    yield return StartCoroutine(DoWait(token.paramList));
                    break;
                    
                case TokenType.WaitForInputNoClear:
                    yield return StartCoroutine(DoWaitForInput(false));
                    break;
                    
                case TokenType.WaitForInputAndClear:
                    yield return StartCoroutine(DoWaitForInput(true));
                    break;

                case TokenType.WaitForVoiceOver:
                    yield return StartCoroutine(DoWaitVO());
                    break;

                    case TokenType.WaitOnPunctuationStart:
                    TryGetSingleParam(token.paramList, 0, punctuationPause, out currentPunctuationPause);
                    break;
                    
                case TokenType.WaitOnPunctuationEnd:
                    currentPunctuationPause = punctuationPause;
                    break;
                    
                case TokenType.Clear:
                        text.text = string.Empty;
                    break;
                    
                case TokenType.SpeedStart:
                    TryGetSingleParam(token.paramList, 0, writingSpeed, out currentWritingSpeed);
                    break;
                    
                case TokenType.SpeedEnd:
                    currentWritingSpeed = writingSpeed;
                    break;
                    
                case TokenType.Exit:
                    exitFlag = true;
                    break;

                case TokenType.Message:
                    if (CheckParamCount(token.paramList, 1)) 
                    {
                        Flowchart.BroadcastFungusMessage(token.paramList[0]);
                    }
                    break;
                    
                case TokenType.VerticalPunch: 
                    {
                        float vintensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out vintensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(0, vintensity, 0), time);
                    }
                    break;
                    
                case TokenType.HorizontalPunch: 
                    {
                        float hintensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out hintensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(hintensity, 0, 0), time);
                    }
                    break;
                    
                case TokenType.Punch: 
                    {
                        float intensity;
                        float time;
                        TryGetSingleParam(token.paramList, 0, 10.0f, out intensity);
                        TryGetSingleParam(token.paramList, 1, 0.5f, out time);
                        Punch(new Vector3(intensity, intensity, 0), time);
                    }
                    break;
                    
                case TokenType.Flash:
                    float flashDuration;
                    TryGetSingleParam(token.paramList, 0, 0.2f, out flashDuration);
                    Flash(flashDuration);
                    break;

                case TokenType.Audio: 
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1))
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                    }
                    break;
                    
                case TokenType.AudioLoop:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Play();
                            audioSource.loop = true;
                        }
                    }
                    break;
                    
                case TokenType.AudioPause:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Pause();
                        }
                    }
                    break;
                    
                case TokenType.AudioStop:
                    {
                        AudioSource audioSource = null;
                        if (CheckParamCount(token.paramList, 1)) 
                        {
                            audioSource = FindAudio(token.paramList[0]);
                        }
                        if (audioSource != null)
                        {
                            audioSource.Stop();
                        }
                    }
                    break;
                }

                previousTokenType = token.type;

                if (exitFlag)
                {
                    break;
                }
            }

            inputFlag = false;
            exitFlag = false;
            isWaitingForInput = false;
            isWriting = false;

            NotifyEnd(stopAudio);

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual IEnumerator DoWords(List<string> paramList, TokenType previousTokenType)
        {
            if (!CheckParamCount(paramList, 1))
            {
                yield break;
            }

            var param = paramList[0].Replace("\\n", "\n");

            // Trim whitespace after a {wc} or {c} tag
            if (previousTokenType == TokenType.WaitForInputAndClear ||
                previousTokenType == TokenType.Clear)
            {
                param = param.TrimStart(' ', '\t', '\r', '\n');
            }

            var textInfo =  text.textInfo; 
            
            // Start with the visible portion of any existing displayed text.
            var startText = "";
            if (visibleCharacterCount > 0 &&
                visibleCharacterCount <= textInfo.characterCount)
            {
                startText = text.text.Substring(0, visibleCharacterCount);
            }
                
            UpdateOpenMarkup();
            UpdateCloseMarkup();

            var timeAccumulator = Time.unscaledDeltaTime;
            var invWritingSpeed = 1f / currentWritingSpeed;

            //pausing for 1 frame means we can get better first data, but is conflicting with animation ?
            //  or is it something else inserting the color alpha invis 
            yield return null;
            //this works for first thing being shown but then no subsequent, as the char counts have not been update
            // by tmpro after the set to ""
            var startingReveal = Mathf.Min(readAheadStartIndex, textInfo.characterCount);
            PartitionString(writeWholeWords, param, param.Length + 1);

            ConcatenateString(startText);
            text.text = outputString.ToString();

            text.maxVisibleCharacters = startingReveal;
            yield return null;

            while (text.maxVisibleCharacters < Mathf.Min(readAheadStartIndex, textInfo.characterCount))
            {
                // No delay if user has clicked and Instant Complete is enabled
                if (instantComplete && inputFlag)
                    text.maxVisibleCharacters = textInfo.characterCount;

                text.maxVisibleCharacters ++;

                var revealed = (text.textInfo.characterInfo.Length > text.maxVisibleCharacters ? text.textInfo.characterInfo[text.maxVisibleCharacters] : text.textInfo.characterInfo.Last()).character;
                NotifyGlyph(text.maxVisibleCharacters);

                // Punctuation pause
                if (IsPunctuation(revealed))
                {
                    yield return StartCoroutine(DoWait(currentPunctuationPause));
                }

                if (currentWritingSpeed > 0f)
                {
                    timeAccumulator -= invWritingSpeed;
                    if (timeAccumulator <= 0f)
                    {
                        var waitTime = Mathf.Max(invWritingSpeed, Time.unscaledDeltaTime);
                        yield return new WaitForSecondsRealtime(waitTime);
                        timeAccumulator += waitTime;
                    }
                }
            }
        }

        protected virtual void PartitionString(bool wholeWords, string inputString, int i)
        {
            leftString.Length = 0;
            rightString.Length = 0;

            // Reached last character
            leftString.Append(inputString);
            if (i >= inputString.Length)
            {
                return;
            }

            rightString.Append(inputString);

            if (wholeWords)
            {
                // Look ahead to find next whitespace or end of string
                for (var j = i; j < inputString.Length + 1; ++j)
                {
                    if (j == inputString.Length || char.IsWhiteSpace(inputString[j]))
                    {
                        leftString.Length = j;
                        rightString.Remove(0, j);
                        break;
                    }
                }
            }
            else
            {
                leftString.Remove(i, inputString.Length - i);
                rightString.Remove(0, i);
            }
        }

        protected virtual void ConcatenateString(string startText)
        {
            outputString.Length = 0;
            readAheadStartIndex = int.MaxValue;

            // string tempText = startText + openText + leftText + closeText;
            outputString.Append(startText);
            outputString.Append(openString);
            outputString.Append(leftString);
            outputString.Append(closeString);

            // Track how many visible characters are currently displayed so
            // we can easily extract the visible portion again later.
            visibleCharacterCount = outputString.Length;

            // Make right hand side text hidden
            if (text.richText && rightString.Length + readAheadString.Length > 0)
            {
                // Ensure the hidden color strings are populated
                if (hiddenColorOpen.Length == 0)
                {
                    CacheHiddenColorStrings();
                }

                readAheadStartIndex = outputString.Length;

                outputString.Append(hiddenColorOpen);
                outputString.Append(rightString);
                outputString.Append(readAheadString);
                outputString.Append(hiddenColorClose);
            }
        }

        protected virtual IEnumerator DoWait(List<string> paramList)
        {
            var param = "";
            if (paramList.Count == 1)
            {
                param = paramList[0];
            }

            var duration = 1f;
            if (!float.TryParse(param, NumberStyles.Any, CultureInfo.InvariantCulture, out duration))
            {
                duration = 1f;
            }

            yield return StartCoroutine( DoWait(duration) );
        }

        protected virtual IEnumerator DoWaitVO()
        {
            var duration = 0f;

            if (AttachedWriterAudio != null)
            {
                duration = AttachedWriterAudio.GetSecondsRemaining();
            }

            yield return StartCoroutine(DoWait(duration));
        }

        protected virtual IEnumerator DoWait(float duration)
        {
            NotifyPause();

            var timeRemaining = duration;
            while (timeRemaining > 0f && !exitFlag)
            {
                if (instantComplete && inputFlag)
                {
                    break;
                }

                timeRemaining -= Time.unscaledDeltaTime;
                yield return null;
            }

            NotifyResume();
        }

        protected virtual IEnumerator DoWaitForInput(bool clear)
        {
            NotifyPause();

            inputFlag = false;
            isWaitingForInput = true;

            while (!inputFlag && !exitFlag)
            {
                yield return null;
            }
        
            isWaitingForInput = false;          
            inputFlag = false;

            if (clear)
            {
                text.text = "";
            }

            NotifyResume();
        }
        
        protected virtual bool IsPunctuation(char character)
        {
            return character == '.' || 
                character == '?' ||  
                    character == '!' || 
                    character == ',' ||
                    character == ':' ||
                    character == ';' ||
                    character == ')';
        }
        
        protected virtual void Punch(Vector3 axis, float time)
        {
            var go = punchObject;
            if (go == null)
            {
                go = Camera.main.gameObject;
            }
        }
        
        protected virtual void Flash(float duration)
        {
            var cameraManager = FungusManager.Instance.CameraManager;

            cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
            cameraManager.Fade(1f, duration, delegate {
                cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
                cameraManager.Fade(0f, duration, null);
            });
        }
        
        protected virtual AudioSource FindAudio(string audioObjectName)
        {
            var go = GameObject.Find(audioObjectName);
            if (go == null)
            {
                return null;
            }
            
            return go.GetComponent<AudioSource>();
        }

        protected virtual void NotifyInput()
        {
            WriterSignals.DoWriterInput(this);

            foreach (var writerListener in writerListeners)
                writerListener.OnInput();
        }

        protected virtual void NotifyStart(AudioClip audioClip)
        {
            WriterSignals.DoWriterState(this, WriterState.Start);

            foreach (var writerListener in writerListeners)
                writerListener.OnStart(audioClip);
        }

        protected virtual void NotifyPause()
        {
            WriterSignals.DoWriterState(this, WriterState.Pause);

            foreach (var writerListener in writerListeners)
                writerListener.OnPause();
        }

        protected virtual void NotifyResume()
        {
            WriterSignals.DoWriterState(this, WriterState.Resume);

            foreach (var writerListener in writerListeners)
                writerListener.OnResume();
        }

        protected virtual void NotifyAllWordsWritten()
        {
            foreach (var writerListener in writerListeners)
                writerListener.OnAllWordsWritten();
        }

        protected virtual void NotifyEnd(bool stopAudio)
        {
            WriterSignals.DoWriterState(this, WriterState.End);

            foreach (var writerListener in writerListeners)
                writerListener.OnEnd(stopAudio);
        }

        protected virtual void NotifyGlyph(int index)
        {
            WriterSignals.DoWriterGlyph(this);

            foreach (var writerListener in writerListeners)
                writerListener.OnGlyph(index);
        }

        #region Public members

        /// <summary>
        /// This property is true when the writer is writing text or waiting (i.e. still processing tokens).
        /// </summary>
        public virtual bool IsWriting { get { return isWriting; } }

        /// <summary>
        /// This property is true when the writer is waiting for user input to continue.
        /// </summary>
        public virtual bool IsWaitingForInput { get { return isWaitingForInput; } }

        /// <summary>
        /// Pauses the writer.
        /// </summary>
        public virtual bool Paused { set; get; }

        /// <summary>
        /// Stop writing text.
        /// </summary>
        public virtual void Stop()
        {
            if (isWriting || isWaitingForInput)
            {
                exitFlag = true;
            }
        }

        /// <summary>
        /// Writes text using a typewriter effect to a UI text object.
        /// </summary>
        /// <param name="content">Text to be written</param>
        /// <param name="clear">If true clears the previous text.</param>
        /// <param name="waitForInput">Writes the text and then waits for player input before calling onComplete.</param>
        /// <param name="stopAudio">Stops any currently playing audioclip.</param>
        /// <param name="waitForVO">Wait for the Voice over to complete before proceeding</param>
        /// <param name="audioClip">Audio clip to play when text starts writing.</param>
        /// <param name="onComplete">Callback to call when writing is finished.</param>
        public virtual IEnumerator Write(string content, bool clear, bool waitForInput, bool stopAudio, bool waitForVO, AudioClip audioClip, System.Action onComplete)
        {
            if (clear)
            {
                text.text = "";
                visibleCharacterCount = 0;
            }

            // If this clip is null then WriterAudio will play the default sound effect (if any)
            NotifyStart(audioClip);

            var tokenText = TextVariationHandler.SelectVariations(content);
            
            if (waitForInput)
            {
                tokenText += "{wi}";
            }

            if(waitForVO)
            {
                tokenText += "{wvo}";
            }


            var tokens = TextTagParser.Tokenize(tokenText);

            gameObject.SetActive(true);

            yield return StartCoroutine(ProcessTokens(tokens, stopAudio, onComplete));
        }

        public void SetTextColor(Color textColor)
        {
            text.color = textColor;
        }

        public void SetTextAlpha(float textAlpha)
        {
            text.color = text.color.WithA(textAlpha);
        }



        #endregion

        #region IDialogInputListener implementation

        public virtual void OnNextLineEvent()
        {
            inputFlag = true;

            if (isWriting)
            {
                NotifyInput();
            }
        }

        #endregion
    }
}
