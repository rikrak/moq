#if !NETSTANDARD2_1 && !NET6_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter |AttributeTargets.Property | AttributeTargets.ReturnValue,Inherited = false)]
    [ExcludeFromCodeCoverage, DebuggerNonUserCode]
    internal sealed class NotNullAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotNullAttribute"/> class.
        /// </summary>
        public NotNullAttribute() { }
    }
}
#endif