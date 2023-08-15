namespace CoreLib.Module
{
    public class PointerWait : PointerActivator
    {
        protected override PointerId _reloveId() => Pointer.Instance.m_CursorSet.Value.m_Wait._id;

        private void OnEnable()
        {
            _take();
        }
    }
}