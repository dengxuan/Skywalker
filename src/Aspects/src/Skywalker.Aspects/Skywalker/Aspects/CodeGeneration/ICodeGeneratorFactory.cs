using Skywalker.Aspects.Abstractinons;

namespace Skywalker.Aspects
{
    /// <summary>
    /// Defaults implmentation of <see cref="ICodeGeneratorFactory"/>.
    /// </summary>
    public sealed class CodeGeneratorFactory : ICodeGeneratorFactory
    {
        /// <summary>
        /// Creates a new <see cref="ICodeGenerator"/> used to generate interceptable proxy class.
        /// </summary>
        /// <returns>Created <see cref="ICodeGenerator"/> used to generate interceptable proxy class..</returns>
        public ICodeGenerator Create() => new CodeGenerator();
    }
}
