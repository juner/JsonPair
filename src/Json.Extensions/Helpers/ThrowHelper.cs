using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Juners.Json.Extensions.Helpers;
internal static class ThrowHelper
{
    [DoesNotReturn]
    internal static T ThrowNotSupportMemberType<T>(MemberTypes memberType)
        => throw CreateNotSupportMemberTypeException(memberType);
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateNotSupportMemberTypeException(MemberTypes memberType)
        => new InvalidOperationException($"not support memberType:{memberType}");
    [DoesNotReturn]
    internal static void ThrowIgnoreConditionOnValueTypeInvalid() => throw CreateIgnoreConditionOnValueTypeInvalid();
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateIgnoreConditionOnValueTypeInvalid()
        => new InvalidOperationException("ignore condition on value type invalid");
    [DoesNotReturn]
    internal static void ThrowSerializationConverterOnAttributeNotCompatible() => throw CreateSerializationConverterOnAttributeNotCompatible();
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Exception CreateSerializationConverterOnAttributeNotCompatible() => new InvalidOperationException("serialization converter on attribute not compatible");
}
