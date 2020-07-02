using System;
using System.Collections.Generic;
using System.Text;

namespace ParseCollingDicts
  {
  public class EntryData
    {
    public List<GramarGroup> GrmGrps = new List<GramarGroup>();

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Mezcla los datos de la entrada con nuevos datos</summary>
    internal void MergeData( EntryData newEntryData )
      {
      for( int i=0; i < newEntryData.GrmGrps.Count; i++ )
        {
        var grp1 = newEntryData.GrmGrps[i];

        int j = 0;
        for( ; j < GrmGrps.Count; j++ )
          {
          var grp2 = GrmGrps[j];
          if( grp1.Name == grp2.Name )
            {
            grp2.Merge( grp1 );
            break;
            }
          }

        if( j >= GrmGrps.Count ) GrmGrps.Add(grp1);
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Reorna una representación del objeto en forma de cadena de caracteres</summary>
    public override string ToString()
      {
      var s = new StringBuilder( 100*GrmGrps.Count );

      foreach( var Grm in GrmGrps )
        {
        //if( s.Length>0 ) s.Append( " | " );

        var sGrpGrm = Grm.ToString();
        if( sGrpGrm.Length > 0 )
          {
          s.Append( "\r\n" );
          s.Append( sGrpGrm );
          }
        }

      return s.ToString();
      }
    }
  }