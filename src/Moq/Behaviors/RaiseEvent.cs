#nullable enable
// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Moq.Behaviors
{

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
        internal sealed class RaiseEvent : Behavior
    After:
        sealed class RaiseEvent : Behavior
    */
    sealed class RaiseEvent : Behavior

    /* Unmerged change from project 'Moq(netstandard2.0)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */

    /* Unmerged change from project 'Moq(netstandard2.1)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */

    /* Unmerged change from project 'Moq(net6.0)'
    Before:
            private Mock mock;
            private LambdaExpression expression;
            private Delegate eventArgsFunc;
            private object[] eventArgsParams;
    After:
            Mock mock;
            LambdaExpression expression;
            Delegate eventArgsFunc;
            object[] eventArgsParams;
    */
    {
        Mock mock;
        LambdaExpression expression;
        Delegate? eventArgsFunc;
        object[]? eventArgsParams;

        public RaiseEvent(Mock mock, LambdaExpression expression, object[] eventArgsParams) : this(mock, expression, null, eventArgsParams)
        {
        }

        public RaiseEvent(Mock mock, LambdaExpression expression, Delegate eventArgsFunc) : this(mock, expression, eventArgsFunc, null)
        {
        }

        RaiseEvent(Mock mock, LambdaExpression expression, Delegate? eventArgsFunc, object[]? eventArgsParams)
        {
            Debug.Assert(eventArgsFunc != null ^ eventArgsParams != null);

            this.mock = Guard.NotNull(mock);
            this.expression = Guard.NotNull(expression);
            this.eventArgsFunc = eventArgsFunc;
            this.eventArgsParams = eventArgsParams;
        }

        public override void Execute(Invocation invocation)
        {
            object[] args;

            if (this.eventArgsParams != null)
            {
                args = this.eventArgsParams;
            }
            else
            {
                var argsFuncType = this.eventArgsFunc!.GetType();
                var argsList = new List<object>()
                {
                    this.mock.Object
                };
                if (argsFuncType.IsGenericType && argsFuncType.GetGenericArguments().Length == 1)
                {
                    var eventArgs = this.eventArgsFunc.InvokePreserveStack();
                    if (eventArgs != null)
                    {
                        argsList.Add(eventArgs);
                    }
                }
                else
                {
                    var eventArgs = this.eventArgsFunc.InvokePreserveStack(invocation.Arguments);
                    if (eventArgs != null)
                    {
                        argsList.Add(eventArgs);
                    }
                }
                args = argsList.ToArray();
            }

            Mock.RaiseEvent(this.mock, this.expression, this.expression.Split(), args);
        }
    }
}
