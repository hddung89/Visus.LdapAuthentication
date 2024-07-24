﻿// <copyright file="DistinguishedNameAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Reflection;


namespace Visus.Ldap.Mapping {

    /// <summary>
    /// Annotates a property of a LDAP user or group object as holding the
    /// distinguished name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = false,
        Inherited = false)]
    public sealed class DistinguishedNameAttribute : Attribute {

        /// <summary>
        /// Gets the only property in <typeparamref name="TType"/> that is
        /// annotated with <see cref="DistinguishedNameAttribute"/>.
        /// </summary>
        /// <remarks>
        /// The method will return nothing if multiple properties are annotated
        /// as DN or if the property is not a writable <see cref="string"/>.
        /// </remarks>
        /// <typeparam name="TType">The type to get the property for.</typeparam>
        /// <returns>The property annotated as DN or <c>null</c> if no unique
        /// attribute was found.</returns>
        public static PropertyInfo? GetProperty<TType>()
            => typeof(TType).GetProperty<DistinguishedNameAttribute>(
                p => (p.PropertyType == typeof(string))
                && p.CanRead
                && p.CanWrite);
    }
}
