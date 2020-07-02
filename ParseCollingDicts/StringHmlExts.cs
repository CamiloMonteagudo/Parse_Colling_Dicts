using System.Collections.Generic;

namespace ParseCollingDicts
  {
  ///-----------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Guarda información sobre un tag html</summary>
  public class HtmlTag
    {
    public string Name;             // Nombre del tag
    public int Close;               // 0 -Tag inicial, 1- Tag final, 2 - Tag Auto cerrado

    public string Txt;              // Definición del tag

    public bool IsTag => ( Name.Length>0 );
    public bool IsTxt => ( Name.Length==0 );
    }

  ///-----------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Implementa extensiones a la clase string, para manejar un texto HTML</summary>
  public static class StringHmlExts
    {
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el contenido del tab 'sTab' dentro de la cadena, si no lo encuentra retorna null</summary>
    public static List<HtmlTag> TagContent( this string s, string sTag )
      {
      var lst = new List<HtmlTag>();

      int idx = s.IndexOf( sTag );
      if( idx==-1 ) return lst;

      var Tag = s.NextTag( ref idx );
      if( Tag==null || Tag.Close != 0 ) return lst;

      int level = 1;
      for(;;)
        {
        var Next = s.NextTag( ref idx );
        if( Next != null )
          {
          if( Next.Name == Tag.Name )
            {
            if( Next.Close==0 ) ++level;
            if( Next.Close==1 ) --level;

            if( level==0 ) return lst;
            }

          lst.Add( Next );
          }
        else
          {
          HtmlTag txt = s.NexText( ref idx );
          if( txt == null ) return new List<HtmlTag>();

          lst.Add( txt );
          }
        }
      }

    static char[] Cascara = { ' ', '\t', '\n', '\r' };
    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el texto que hay entre dos tags</summary>
    private static HtmlTag NexText( this string s, ref int idx )
      {
      s.SkipCtlChars( ref idx );                                    // Salta espacios, tabs y cambio de linea

      int iTxt = idx;
      while( idx<s.Length && s[idx]!='<' ) ++idx;                   // Salta hasta incio del proximo tag
      if( iTxt == idx ) return null;                                // No hay ningún caracter como nombre

      var txt = new HtmlTag();
      txt.Txt = s.Substring( iTxt, idx-iTxt ).TrimEnd(Cascara);     // Extrae el nombre del tag
      txt.Name = "";

      return txt;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el contenido del tab 'sTab' dentro de la cadena, si no lo encuentra retorna null</summary>
    public static HtmlTag NextTag( this string s, ref int idx )
      {
      var tag = new HtmlTag();

      s.SkipCtlChars( ref idx );                                    // Salta espacios, tabs y cambio de linea

      if( s[idx] != '<' ) return null;                              // Verifica que es un inicio de tag
      var iTag = idx++;                                             // Marca inicio del tag y lo salta

      if( s[idx] == '/' )                                           // Si es un tag de cierre
        {
        tag.Close = 1;
        ++idx;
        }

      s.SkipCtlChars( ref idx );                                    // Salta espacios

      var iName = idx;                                              // Inicio del nombre del tag

      while( idx<s.Length && char.IsLetterOrDigit( s, idx ) ) ++idx;       // Salta caracteres del nombre del tag    
      if( iName == idx ) return null;                               // No hay ningún caracter como nombre

      tag.Name = s.Substring( iName, idx-iName );                   // Extrae el nombre del tag

      while( idx<s.Length && s[idx]!='>' ) ++idx;                   // Salta hasta el final del tag

      if( s[idx-1] == '/' )  tag.Close = 2;                         // No necesita tag de cierre

      ++idx;
      tag.Txt = s.Substring( iTag, idx-iTag );                      // Extrae texto del tag

      return tag;
      }

    //------------------------------------------------------------------------------------------------------------------
    ///<summary>Obtiene el nombre del TAG de la cadena s </summary>
    private static string GetTagName( this string s, ref int idx )
      {
      s.SkipCtlChars(ref idx);                                      // Salta espacios, tabs y cambio de linea

      if( s[idx] != '<' ) return null;                              // Verifica que es un inicio de tag
      ++idx;                                                        // Salta el marcador de comienzo

      s.SkipCtlChars( ref idx );                                    // Salta espacios

      var iIni = idx;                                               // Inicio del nombre del tag

      while( idx<s.Length && char.IsLetter(s,idx) ) ++idx;          // Salta caracteres del nombre del tag    
      if( iIni == idx ) return null;                                // No hay ningún caracter como nombre

      return s.Substring( iIni, idx-iIni);                          // Extrae el nombre y lo retorna
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta los caracteres de control a partir de 'Idx'</summary>
    public static void SkipCtlChars( this string s, ref int Idx )
      {
      while( Idx<s.Length && s[Idx]<=' ' ) ++Idx;                   // Salta espacios y caracteres de control
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Quita de la cadena todo el contenido que este entre 'cIni' y 'cEnd'</summary>
    public static string RemoveEntre( this string s, char cIni, char cEnd )
      {
      var iIni = s.IndexOf(cIni);
      if( iIni==-1 ) return s;

      int level = 1;
      for( int i=iIni+1; i<s.Length; i++ )
        {
        if( s[i]==cEnd ) --level;
        else if( s[i]==cIni )  ++level;

        if( level==0 )
          {
          s = s.Substring(0,iIni) + s.Substring(i+1);
          return s.RemoveEntre(cIni, cEnd);
          }
        }

      return s;
      }
    }
  }
