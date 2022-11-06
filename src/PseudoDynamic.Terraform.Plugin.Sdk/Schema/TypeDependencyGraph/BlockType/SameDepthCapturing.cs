namespace PseudoDynamic.Terraform.Plugin.Schema.TypeDependencyGraph.BlockType
{
    /// <summary>
    /// A specialized class for capturing children of one visitation.
    /// This technique can be used to transform a recursive visitors
    /// into a declarative visitor by capturing children of a
    /// visitation and then process these children declarative.
    /// Used mainly for grouping children and adding them to a parent
    /// node before proceeding with their children.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SameDepthCapturing<T>
    {
        public bool IsSameDepthCapturingEnabled => _queueSameDepth;

        private bool _queueSameDepth;
        private readonly Queue<T> _sameDepthCaptured = new();

        public bool EnableSameDepthCapturing(bool enable = true) => _queueSameDepth = enable;

        public void CaptureSameDepth(T value) => _sameDepthCaptured.Enqueue(value);

        public List<T> CaptureSameDepth(Action<T> visitSameDepth, T visitSameDepthArgument)
        {
            EnableSameDepthCapturing();
            visitSameDepth(visitSameDepthArgument);
            EnableSameDepthCapturing(false);

            List<T> capturedContexts = new(_sameDepthCaptured);
            _sameDepthCaptured.Clear();
            return capturedContexts;
        }
    }
}
