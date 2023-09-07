#nullable enable
// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq.Polyfill;
using Moq.Properties;

using TypeNameFormatter;

namespace Moq
{
    [DebuggerStepThrough]
    static class Guard
    {
        public static void CanCreateInstance(Type type)
        {
            if (!type.CanCreateInstance())
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypeHasNoDefaultConstructor,
                        type.GetFormattedName()));
            }
        }

        public static void ImplementsInterface(Type interfaceType, Type type, string? paramName = null)
        {
            NotNull(interfaceType);
            Debug.Assert(interfaceType.IsInterface);

            NotNull(type);

            if (interfaceType.IsAssignableFrom(type) == false)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypeNotImplementInterface,
                        type.GetFormattedName(),
                        interfaceType.GetFormattedName()),
                    paramName);
            }
        }

        public static void ImplementsTypeMatcherProtocol(Type type)
        {
            NotNull(type);

            Guard.ImplementsInterface(typeof(ITypeMatcher), type);
            Guard.CanCreateInstance(type);
        }

        public static void IsAssignmentToPropertyOrIndexer(LambdaExpression expression, string paramName)
        {
            NotNull(expression);

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Assign:
                    var assignment = (BinaryExpression)expression.Body;
                    if (assignment.Left is MemberExpression || assignment.Left is IndexExpression) return;
                    break;

                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression.Body;
                    if (call.Method.IsSetAccessor()) return;
                    break;
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.SetupNotSetter,
                    expression.ToStringFixed()),
                paramName);
        }

        public static void IsOverridable(MethodInfo method, Expression expression)
        {
            if (method.IsStatic)
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.UnsupportedExpressionWithHint,
                        expression.ToStringFixed(),
                        string.Format(
                            CultureInfo.CurrentCulture,
                            method.IsExtensionMethod() ? Resources.UnsupportedExtensionMethod : Resources.UnsupportedStaticMember,
                            $"{method.DeclaringType?.GetFormattedName()}.{method.Name}")));
            }
            else if (!method.CanOverride())
            {
                throw new NotSupportedException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.UnsupportedExpressionWithHint,
                        expression.ToStringFixed(),
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.UnsupportedNonOverridableMember,
                            $"{method.DeclaringType?.GetFormattedName()}.{method.Name}")));
            }
        }

        public static void IsVisibleToProxyFactory(MethodInfo method)
        {
            if (ProxyFactory.Instance.IsMethodVisible(method, out string messageIfNotVisible) == false)
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.MethodNotVisibleToProxyFactory,
                    method.DeclaringType?.Name ?? string.Empty,
                    method.Name,
                    messageIfNotVisible));
            }
        }

        public static void IsEventAdd(LambdaExpression expression, string paramName)
        {
            NotNull(expression);

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression.Body;
                    if (call.Method.IsEventAddAccessor()) return;
                    break;
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.SetupNotEventAdd,
                    expression.ToStringFixed()),
                paramName);
        }

        public static void IsEventRemove(LambdaExpression expression, string paramName)
        {
            NotNull(expression);

            switch (expression.Body.NodeType)
            {
                case ExpressionType.Call:
                    var call = (MethodCallExpression)expression.Body;
                    if (call.Method.IsEventRemoveAccessor()) return;
                    break;
            }

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.SetupNotEventRemove,
                    expression.ToStringFixed()),
                paramName);
        }

        /// <summary>
        /// Ensures the given <paramref name="value"/> is not null.
        /// Throws <see cref="ArgumentNullException"/> otherwise.
        /// </summary>
        [return: NotNullIfNotNull("value")]
        public static T NotNull<T>([NotNull][ValidatedNotNull]T? value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            if (IsNull(value))
            {
                ThrowOnNull(paramName);
            }
            return value;
        }

        static bool IsNull<T>([NotNullWhen(false)] T? value) => value is null;

        [DoesNotReturn]
        static void ThrowOnNull(string? paramName) => throw new ArgumentNullException(paramName ?? "<unknown>");

        /// <summary>
        /// Ensures the given string <paramref name="value"/> is not null or empty.
        /// Throws <see cref="ArgumentNullException"/> in the first case, or 
        /// <see cref="ArgumentException"/> in the latter.
        /// </summary>
        public static void NotNullOrEmpty([ValidatedNotNull] string? value, [CallerArgumentExpression("value")] string? paramName = null)
        {
            NotNull(value, paramName);

            if (value.Length == 0)
            {
                throw new ArgumentException(Resources.ArgumentCannotBeEmpty, paramName);
            }
        }

        public static void NotField(MemberExpression memberAccess)
        {
            if (memberAccess.Member is FieldInfo)
                throw new NotSupportedException(
                    string.Format(
                        Resources.FieldsNotSupported,
                        memberAccess.ToStringFixed()));
        }

        public static void IsMockable(Type type)
        {
            if (!type.IsMockable())
            {
                throw new NotSupportedException(
                    string.Format(
                        Resources.TypeNotMockable,
                        type.GetFormattedName()));
            }
        }

        public static void Positive(TimeSpan delay)
        {
            if (delay <= TimeSpan.Zero)
            {
                throw new ArgumentException(Resources.DelaysMustBeGreaterThanZero);
            }
        }

        public static void CanRead(PropertyInfo property)
        {
            if (!property.CanRead(out _))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PropertyGetNotFound,
                    property.DeclaringType?.Name ?? string.Empty,
                    property.Name));
            }
        }

        public static void CanWrite(PropertyInfo property)
        {
            if (!property.CanWrite(out _))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.PropertySetNotFound,
                    property.DeclaringType?.Name ?? string.Empty,
                    property.Name));
            }
        }
    }
}
