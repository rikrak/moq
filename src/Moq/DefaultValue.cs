#nullable enable
// Copyright (c) 2007, Clarius Consulting, Manas Technology Solutions, InSTEDD, and Contributors.
// All rights reserved. Licensed under the BSD 3-Clause License; see License.txt.

using System.ComponentModel;

namespace Moq
{
    /// <summary>
    /// Determines the way default values are generated 
    /// calculated for loose mocks.
    /// </summary>
    public enum DefaultValue
    {
        /// <summary>
        /// Default behavior, which generates empty values for 
        /// value types (i.e. default(int)), empty array and 
        /// enumerables, and nulls for all other reference types.
        /// </summary>
        Empty,

        /// <summary>
        /// Whenever the default value generated by <see cref="Empty"/> 
        /// is null, replaces this value with a mock (if the type 
        /// can be mocked). 
        /// </summary>
        /// <remarks>
        /// For sealed classes, a null value will be generated.
        /// </remarks>
        Mock,

        /// <summary>
        ///   <para>
        ///     All default value generation strategies other than <see cref="Empty"/> or <see cref="Mock"/>
        ///     are represented by this enumeration value.
        ///   </para>
        ///   <para>
        ///     Do not set <see cref="Mock.DefaultValue"/> (nor <see cref="MockFactory.DefaultValue"/>) to this value.
        ///     If you want to set up a custom default value generation strategy, set <see cref="Mock.DefaultValueProvider"/>
        ///     or <see cref="MockFactory.DefaultValueProvider"/> instead.
        ///   </para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Custom
    }
}
