var HandleIO = 
{
    WindowAlert : function(message)
    {
        window.alert(Pointer_stringify(message));
    },
    SyncFiles : function()
    {
        FS.syncfs(false,function (err) 
        {
            // handle callback
        });
    },
    WebURI : function()
    {
        return Document.documentURI;
    }
};

mergeInto(LibraryManager.library, HandleIO);