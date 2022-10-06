using PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors.Generic;

namespace PseudoDynamic.Terraform.Plugin.Schema.TypeVisitors
{
    internal class BlockTypeNodeProducer
    {
        private BlockTypeVisitor _visistor = new BlockTypeVisitor();

        /// <summary>
        /// Represents the entry point of visiting a complex type.
        /// </summary>
        /// <param name="blockType"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentException"></exception>
        public virtual BlockTypeNode Produce(Type blockType, RootVisitContext? context = null)
        {
            if (!blockType.IsComplexType()) {
                throw new ArgumentException("The specified type must be either class or struct");
            }

            _visistor.VisitComplex(blockType);
            return _visistor.RootNode;
        }

        public BlockTypeNode Produce<T>(RootVisitContext? context = null) =>
            Produce(typeof(T), context);
    }
}
