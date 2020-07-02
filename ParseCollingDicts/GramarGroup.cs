using System;
using System.Collections.Generic;
using System.Text;

namespace ParseCollingDicts
  {
  public class GramarGroup
    {
    public string Name;
    public List<Mean> Means = new List<Mean>();

    public GramarGroup( string grmTipo ) => Name=grmTipo;

    internal void AddMean( string sTrd )
      {
      Means.Add( new Mean(sTrd) );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla los datos de un grupo gramatical con nuevos datos</summary>
    public void Merge( GramarGroup grp )
      {
      for( int i=0; i < grp.Means.Count; i++ )
        {
        var mean1 = grp.Means[i];

        AddIfNoExist( mean1 );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Adciona el significado 'sMean' a la lista si no ha sido adicionado anteriormente</summary>
    internal bool AddIfNoExist( Mean mean )
      {
      for( int j = 0; j < Means.Count; j++ )
        if( Means[j].sMean == mean.sMean ) return false;

      Means.Add( mean );
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Retorna una representación del objeto en forma de cadena de caracteres</summary>
    public override string ToString()
      {
      var s = new StringBuilder( 100*Means.Count );

      if( !string.IsNullOrWhiteSpace(Name) && Means.Count>0 )
        {
        s.Append( Name );
        s.Append( ": " );
        }

      for( int i = 0; i<Means.Count; i++ )
        {
        if( i>0 ) s.Append( ", " );

        s.Append( Means[i].ToString() );
        }

      return s.ToString();
      }

    }
  }