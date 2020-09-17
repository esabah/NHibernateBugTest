using System;
using System.Collections.Generic;
using System.Text;
using DbConnectionTest.Session;

namespace DbConnectionTest.Session
{

    [Serializable]
    public class Tupple<TObject> : IEquatable<TObject>, ITupple where TObject : class
    {
        #region Constants

        private static readonly Func<TObject, int> GetHashCodeMethod = EqualityFunctionsGenerator<TObject>
            .CreateGetHashCode();
        private static readonly Func<TObject, TObject, bool> EqualsMethod = EqualityFunctionsGenerator<TObject>
            .CreateEqualityComparer();

        private static readonly Func<TObject, string> ToStringMethod = EqualityFunctionsGenerator<TObject>
            .CreateToString();

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as TObject);
        }

        public override int GetHashCode()
        {
            var @this = ((object)this) as TObject;

            if (@this == null)
            {
                return 0;
            }

            return GetHashCodeMethod(@this);
        }
        public override string ToString()
        {
            var @this = ((object)this) as TObject;

            if (@this == null)
            {
                return null;
            }

            return ToStringMethod(@this);
        }
        #endregion

        #region IEquatable<TEntity> Members

        public bool Equals(TObject other)
        {
            var @this = ((object)this) as TObject;

            if (other == null || @this == null)
            {
                return false;
            }

            return EqualsMethod(@this, other);
        }

        #endregion
    }
}