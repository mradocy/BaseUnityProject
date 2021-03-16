using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Unity;
using System;
using System.Linq.Expressions;

namespace Core.Unity {

    /// <summary>
    /// Provides a quick way to cast from one type to another.
    /// </summary>
    /// <typeparam name="TTarget">Target type</typeparam>
    /// <remarks>From https://stackoverflow.com/a/23391746</remarks>
    public static class CastTo<TTarget> {

        /// <summary>
        /// Casts <see cref="TSource"/> to <see cref="TTarget"/>.
        /// This does not cause boxing for value types.
        /// Useful in generic methods, e.g. casting a generic enum to an int.
        /// </summary>
        /// <typeparam name="TSource">Source type to cast from. Usually a generic type.</typeparam>
        public static TTarget From<TSource>(TSource s) {
            return Cache<TSource>.Caster(s);
        }

        private static class Cache<TSource> {
            public static readonly Func<TSource, TTarget> Caster = Get();

            private static Func<TSource, TTarget> Get() {
                ParameterExpression p = Expression.Parameter(typeof(TSource));
                UnaryExpression c = Expression.ConvertChecked(p, typeof(TTarget));
                return Expression.Lambda<Func<TSource, TTarget>>(c, p).Compile();
            }
        }

    }
}