namespace UnityEditor.XCodeEditor
{
    using System.Collections;

    public class PBXList : ArrayList
    {
        public PBXList()
        {
        }

        public PBXList( object firstValue )
        {
            this.Add( firstValue );
        }
    }

//  public class PBXList<T> : ArrayList
//  {
//      public int Add( T value )
//      {
//          return (ArrayList)this.Add( value );
//      }
//  }
}
