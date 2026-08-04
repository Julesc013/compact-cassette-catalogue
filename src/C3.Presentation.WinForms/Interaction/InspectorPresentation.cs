namespace C3.Presentation.WinForms.Interaction
{
    public sealed class InspectorPresentation<T>
        where T : class
    {
        public T Value { get; private set; }

        public bool HasSelection => Value != null;

        public void Select(T value)
        {
            Value = value;
        }

        public void Clear()
        {
            Value = null;
        }
    }
}
