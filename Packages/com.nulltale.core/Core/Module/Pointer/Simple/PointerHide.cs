namespace CoreLib.Module
{
    public class PointerHide : PointerActivator
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_None._id;

        private void OnEnable()
        {
            _take();
        }
    }
}