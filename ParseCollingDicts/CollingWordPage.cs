using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseCollingDicts
  {
  //------------------------------------------------------------------------------------------------------------------
  ///<summary>Delegado para resivir notificación del proceso se analisis de la página</summary>
  public delegate void ParseMsgs( string msg );

  ///-----------------------------------------------------------------------------------------------------------------------------------
  /// <summary>Analiza una pagina web que define una entrada de un diccionario de Colling y extrae infomación</summary>
  public class CollingWordPage
    {
    const string mkWrdData = "<div class=\"definition_content res_cell_center_content\">";
    List<HtmlTag> Tags = new List<HtmlTag>();

    ParseMsgs Msgs;
    string fileName;

    string nowEntryKey;
    EntryData nowEntryData;
    GramarGroup nowGrmTipo;

    Dictionary<string, EntryData> Entries;

    static char[] Cascara = { ' ', '\t', '\n', '\r' };

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea el objeto y carga la página para iniciar el analisis</summary>
    public CollingWordPage( string filePath, ParseMsgs msgs=null )
      {
      fileName = Path.GetFileName( filePath );
      Msgs = msgs;

      string sHtml;
      try { sHtml = File.ReadAllText( filePath ); }
      catch( Exception e)
        { 
        Msgs?.Invoke( "Error cargando la página: " + e.Message ); 
        return;
        }

      Tags = sHtml.TagContent( mkWrdData );
      if( Tags.Count == 0 )
        Msgs?.Invoke( "No sección de datos --> " + fileName );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza las secciones fundamentales de la página</summary>
    public bool ParsePageSeccions( Dictionary<string, EntryData> dict )
      {
      Entries = dict;

      int idx = 0;                                                            // Primer tag de la sección de datos
      if( !SkipTag( "h1", ref idx, "No encuentra el título de la página" ) )  // Salta sección de redes sociales
        return false;

      while( idx < Tags.Count )                                               // Recorre todas las secciones de información
        {
        var Tag = Tags[idx];                                                  // Toma el tag que inicia la sección
        int iIni = idx+1;

        if( Tag.Name == "script" )                                            // Si el tag es un script, lo salta
          {
          SkipTag( "script", ref idx );
          continue;
          }

        if( !SkipTag( "div", ref idx ) )                                      // Salta la seccion actual
          {
          setMsg( "No se encontro una sección de información" );
          return false;
          }

        var sTag = Tag.Txt;                                                 // Toma el tag que inicia la sección
        if( sTag.StartsWith( "<div class=\"homograph-entry\"" ) )
          {
          int iEnd = idx-1;

          if( Tags[iIni].Txt == "<div class=\"page\">" )
            { 
            ++iIni; --iEnd; 
            while( Tags[iIni].Txt.IndexOf( "class=\"content-box content-box-definition dictionary biling\"" ) >= 0 )
              {
              int _iIni = iIni;
              if( !SkipTag( "div", ref iIni ) ) break;

              if( !ParseEntry( _iIni, iIni ) ) return false;
              }

            if( iIni != iEnd )
              {
              setMsg( "No analizo la página completa" );
              return false;
              }
            }
          else
            {
            if( !ParseEntry( iIni, iEnd ) ) return false;
            }
          }
        else if( sTag.StartsWith( "<div class=\"content-box content-box-examples\"" ) )
          {

          }
        else if( sTag.StartsWith( "<div class=\"content-box content-box-nearby-words\"" ) )
          {

          }
        else if( sTag.StartsWith( "<div class=\"content-box content-box-origin\"" ) )
          {

          }
        else if( sTag.StartsWith( "<div class=\"content-box content-box-usage\"" ) )
          {

          }
        else if( sTag.StartsWith( "<div class=\"content-box content-box-translations\"" ) )
          {

          }
        else if( sTag.IndexOf( "class=\"content-box content-box-images\"" )>=0 )
          {

          }
        else if( sTag.IndexOf( "class=\"content-box content-box-videos\"" )>=0 )
          {

          }
        else if( sTag.StartsWith( "<div class=\"btmslot_a-container\"" ) )
          {

          }
        else
          {
          setMsg( "Sección no reconocida: " + sTag );
          }
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza las secciones fundamentales de la página</summary>
    public bool ParseEntry( int idx, int iEnd )
      {
      InitializeEntry();

      if( !ParseTrdsHeader( ref idx ) ) return false;                     // Procesa encabezamiento de la seccion de traducción

      SkipSocialButtons( ref idx );                                       // Salta información de redes sociales (si la hay)
      ParseInflexiones( ref idx );
      ParseSonido( ref idx );                                             // Si hay información de sonido la analiza
      ParseSemantica( ref idx );                                          // Si hay informacion semantica la analiza
      int nFrases = ParseFrases( ref idx, 1 );                            // Analiza las frases (Si las hay)
      SkipBlocks( "<span class=\"minimalunit", ref idx );                 // Salta todos los bloques de ese tipo
      var info = SkipParentisis( ref idx );                               // Obtiene información entre parentisis (Si hay)

      int nTrd = 0;                                                       // Trata de obtener las traduciones, por los formatos posibles
      while( ParseTrdInfoByGrmTipo( ref idx )     ||
             ParseTrdListLavel1( ref idx )      ||
             ParseTrdInfoByBlockBiling( ref idx ) ||
             ParseTrdInfoByBlockHom( ref idx )    ||
             ParseReferByGrmTipo( ref idx )       ||
             nFrases>0                            ) 
        {
        ++nTrd;
        nFrases = 0;
        ParseFrases( ref idx, 2 );                                          // Analiza las frases (Si las hay)
        }

      if( nTrd==0 )
        {
        setMsg( "No se puedo obtener ninguna traducción." );
        return false;
        }

      SkipIsVacio( ref idx );                                             // Salta bloque actual si esta vacio

      if( Tags[idx].Txt=="<h3 class=\"h3_entry\">" )                      // Si tiene la sección de palabras compuesta (La ignora)
        {
        SkipTag( "h3", ref idx );                                         // Salta el titulo
        SkipTag( "ul", ref idx );                                         // Salta definición de compuestas
        }

      ParseSemantica( ref idx );                                          // Si hay informacion semantica la analiza

      SkipBlocks( "<div class=\"cit cit-type-example\">", ref idx );      // Salta todos los bloques de ese tipo
      SkipBlocks( "<div class=\"xr hom-subsec\">"       , ref idx );      // Salta todos los bloques de ese tipo
      SkipBlocks( "<span class=\"xr\">"                 , ref idx );      // Salta todos los bloques de ese tipo (Referencia)

      if( Tags[idx].Txt=="Ver" )                                          // Tiene sección ver otras palabras (La ignora)
        {
        ++idx;
        SkipTag( "span", ref idx, "No hay 'SPAN' después de ver" );       // Salta las palabras a ver
        }

      ParseFrases( ref idx, 3 );                                          // Analiza las frases (Si las hay)
      ParseSemantica( ref idx );                                          // Si hay informacion semantica la analiza

      SkipBlocks( "<div class=\"xr hom-subsec\">", ref idx );             // Salta todos los bloques de ese tipo

      if( Tags[idx].Txt.IndexOf( "class='am-default contentslot'" ) >= 0 )
        SkipTag( "div", ref idx );                                        // Salta la sección completa

      SkipBlocks( "<div class=\"content-box content-box-derived\">", ref idx );   // Salta bloques de formas derivadas

      if( !CheckStartTag( "</div>", ref idx, "cierre de seccion content-box-translation" ) ) return false;

      SkipBlocks( "<div class=\"content-box content-box-derived\">", ref idx );   // Salta bloques de formas derivadas

      if( idx != iEnd )
        {
        setMsg( "Falta información por analizar en la sección de traducción" );
        return false;
        }

      if( nowEntryKey == null ) setMsg( "No hay nombre para la entrada" );
      else if( nowEntryData == null ) setMsg( "No hay datos para la para la entrada" );
      //else if( nowEntryData.GrmGrps.Count == 0 ) setMsg( "No hay ningún grupo gramatical en la entrada" );
      else if( Entries.ContainsKey( nowEntryKey ) )
        Entries[nowEntryKey].MergeData( nowEntryData );
      else
        {
        Entries.Add( nowEntryKey, nowEntryData );
        }

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Inicializa los datos para una entrada (limpia los datos anteriores)</summary>
    private void InitializeEntry()
      {
      nowEntryKey  = null;
      nowEntryData = new EntryData();
      nowGrmTipo   = null;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Machea un grupo de referencias marcadas por <span class="gramGrp"></summary>
    private bool ParseReferByGrmTipo( ref int idx )
      {
      if( Tags[idx].Txt!="<span class=\"gramGrp\">" ) return false;
      if( !SkipIsVacio( ref idx ) ) return false;

      int nFrases = ParseFrases( ref idx, 2 );                                          // Analiza las frases (Si las hay)
      return ( nFrases>0);
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta el bloque 'i' si no tiene ningún texto</summary>
    private bool SkipIsVacio( ref int idx )
      {
      if( idx+1 < Tags.Count && Tags[idx].Name == Tags[idx+1].Name && Tags[idx].Close==0 && Tags[idx+1].Close==1 )
        {
        idx +=2;
        return true;
        }

      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones cuando el tipo gramatical viene "suelto"</summary>
    private bool ParseTrdInfoByGrmTipo( ref int i)
      {
      int idx = i;
      var grmTipo = GetH3GrmTipo( ref idx );                              // Obtiene el tipo gramatical (Si hay)
      if( grmTipo==null ) return false;

      nowGrmTipo = new GramarGroup( grmTipo );

      if( ParseFrasesReference( ref idx )>0 )                            // Manada a ver otras palabras o frases
        {
        }
      else if( Tags[idx].Txt.IndexOf( "class=\"xr\"" )>=0 )              // Hace una referencia directa
        {
        SkipTag( Tags[idx].Name, ref idx );
        }
      else if( SkipTag( "ul", ref idx ) )                                // Lista de referencias
        {
        }
      else
        {
        ParseFrases( ref idx, 2 );                                          // Analiza las frases (Si las hay)
        var info = SkipParentisis( ref idx );                               // Obtiene información entre parentisis (Si hay)

        if( !ParseTrdListLavel1( ref idx ) )
          return false;
        }

      nowEntryData.GrmGrps.Add( nowGrmTipo );

      i = idx;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de una lista de significados de primer nivel</summary>
    private bool ParseTrdListLavel1( ref int idx )
      {
      if( Tags[idx].Name != "ol" ) return false;
      ++idx;

      while( Tags[idx].Name == "li" && Tags[idx].Close == 0 )
        {
        ++idx;
        SkipBlocks("<span class=\"drv\">", ref idx);
        SkipBlocks( "<span class=\"lbl tm\">", ref idx );

        while( true )
          {
          var inflx = ParseInflexiones( ref idx );
          var info1 = SkipParentisis( ref idx );                           // Obtiene información entre parentisis (Si hay)

          if( Tags[idx].Txt.Trim()==":" ) ++idx;
          if( Tags[idx].Txt.Trim()=="," ) ++idx;

          if( inflx==0 && info1==null ) break;
          }

        var info2 = SkipGramGrp( ref idx );                              // Obtiene información 
        var tipo  = GetH3GrmTipo( ref idx );
        if( tipo != null )
          {
          nowGrmTipo = new GramarGroup( tipo );
          nowEntryData.GrmGrps.Add( nowGrmTipo );
          }

        var ortho = ParseNotaOrtografica( ref idx );
        SkipBlocks( "<span class=\"lbl tm\">", ref idx );

        int nMeans = 0;
        while(true)
          {
          SkipIsVacio( ref idx);
          if( Tags[idx].Name == "br" ) ++idx;

          var trd = GetTranslation( ref idx );
          var lst  = ParseTrdListLavel2( ref idx );
          var inf  = PaseFrasesEjemAndRefer( ref idx );
          var sem  = ParseSemantica( ref idx );

          if( !trd && !lst && !inf && !sem ) break;

          ++nMeans;
          SkipSeparatorSimple( ref idx );
          }

        if( nMeans == 0 )
          setMsg( "WARNING2: No se encontro información útil, en el significado" );

        CheckCloseTag( "li", ref idx, "WARNING2: No se encontro el final del item de traducción" );
        }

      CheckCloseTag( "ol", ref idx, "WARNING2: No se encontro el final de la lista de traducción" );

      return true; 
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Si hay un separador simple lo salta</summary>
    private bool SkipSeparatorSimple( ref int idx )
      {
      var txt = Tags[idx].Txt.Trim();

      if( Tags[idx].Name != "br" && txt!="," && txt!=":") return false;
        
      ++idx;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene la trducción si la hay y la almacena en la entrada actual del diccionario</summary>
    private bool GetTranslation( ref int idx ) 
      {
      var sTrd = ParseTypeTranslation( ref idx );
      if( sTrd==null ) return false;

      if( nowGrmTipo==null )
        {
        nowGrmTipo = new GramarGroup( "" );
        nowEntryData.GrmGrps.Add( nowGrmTipo );
        }

      nowGrmTipo.AddMean( sTrd );
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza todas las frases, ejemplos o referencias que haya seguidas</summary>
    private bool PaseFrasesEjemAndRefer( ref int idx )
      {
      bool found = false;
      for(;;)
        {
        var nEjempl = ParseEjemplos( ref idx );
        var nFrases = ParseFrases( ref idx, 3 );                                          // Analiza las frases (Si las hay)
        var Referec = GetReferencias( ref idx );
        var Bcks    = SkipBlocks("<span class=\"drv\">", ref idx);
        var info    = SkipParentisis( ref idx );
        var info2   = SkipGramGrp( ref idx );                              // Obtiene información 


        if( nEjempl==0 && nFrases==0 && Referec==null && !Bcks && info==null && info==null ) break;
        found = true;
        } 

      return found;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza el contenido de los ejemplos</summary>
    private int ParseEjemplos( ref int idx )
      {
      int nEjempl = 0;

      for( ; idx<Tags.Count; )
        {
        if( Tags[idx].Txt != "<div class=\"cit cit-type-example\">" &&
            Tags[idx].Txt != "<div class=\"cit type-example\">" ) break;

        SkipTag( "div", ref idx );

        ++nEjempl;
        }

      return nEjempl;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Chequea si en la posición actual esta el cierre del tag 'nTag'</summary>
    private bool CheckCloseTag( string nTag, ref int idx, string Msg )
      {
      if( Tags[idx].Name == nTag && Tags[idx].Close == 1 )
        {
        ++idx;
        return true;
        }

      setMsg( Msg );
      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private string SkipGramGrp( ref int idx )
      {
      string sInfo = "";
      while(Tags[idx].Txt == "<span class=\"gramGrp\">" || Tags[idx].Txt == "<span class=\"lbl mod\">" )
        {
        if( sInfo.Length>0 ) sInfo += ", ";
        sInfo += GetTextTags( ref idx );

        if( Tags[idx].Txt.Trim()=="," || Tags[idx].Txt.Trim()==":" ) ++idx;
        }

      return sInfo;
      }

///-----------------------------------------------------------------------------------------------------------------------------------
/// <summary>Analiza las traducciones de una palabra</summary>
    private string ParseTypeTranslation( ref int idx )
      {
      string sTrd = "";
      if( Tags[idx].Txt == "<span class=\"cit cit-type-translation\">" ) 
        {
        sTrd = GetTextTags( ref idx );
        var info = SkipParentisis( ref idx );                           // Obtiene información entre parentisis (Si hay)
        if( sTrd != null && info != null )
          sTrd += " (" + info + ")";
        }
      else if( Tags[idx].Txt.StartsWith( "<span class=\"cit type-translation" ) )
        {
        sTrd = GetTextTags( ref idx );
        if( ParentisisInfo( ref idx, out string sInfo ) )
          sTrd += ' ' + sInfo;

        if( LabelTypeInfo( ref idx, out string sLabel ) )
          sTrd += ' ' + sLabel;
        }
      else return null;

      return sTrd;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de una lista de significados de segundo nivel</summary>
    private bool ParseTrdListLavel2( ref int idx )
      {
      if( Tags[idx].Name != "ol" ) return false;
      ++idx;

      while( Tags[idx].Name == "li" && Tags[idx].Close == 0 )
        {
        ++idx;
        SkipBlocks( "<span class=\"drv\">", ref idx );
        SkipBlocks( "<span class=\"lbl tm\">", ref idx );

        var info = "";
        while( true )
          {
          var inflx = ParseInflexiones( ref idx );
          var info1 = SkipParentisis( ref idx );                           // Obtiene información entre parentisis (Si hay)

          if( Tags[idx].Txt.Trim()==":" ) ++idx;
          if( Tags[idx].Txt.Trim()=="," ) ++idx;

          if( inflx==0 && info1==null ) break;
          info = info1;
          }

        var info2 = SkipGramGrp( ref idx );                              // Obtiene información 
        var tipo  = GetH3GrmTipo( ref idx );
        if( tipo != null )
          {
          setMsg( "Un tipo gramatical dentro de una lista de nivel2" );
          }

        var ortho = ParseNotaOrtografica( ref idx );
        SkipBlocks( "<span class=\"lbl tm\">", ref idx );
        SkipBlocks( "<span class=\"lbl sense\">", ref idx );

        int nMeans = 0;
        while( true )
          {
          SkipIsVacio( ref idx );

          var trd = GetTranslation( ref idx );
          var inf  = PaseFrasesEjemAndRefer( ref idx );
          var lst  = ParseTrdListLavel2( ref idx );
          var sem  = ParseSemantica( ref idx );

          if( !trd && !inf && !lst && !sem ) break;

          ++nMeans;
          if( Tags[idx].Name == "br" ) ++idx;
          if( Tags[idx].Txt.Trim()=="," ) ++idx;
          if( Tags[idx].Txt.Trim()==":" ) ++idx;
          }

        if( nMeans == 0 )
          setMsg( "WARNING2: No se encontro información útil, en el significado" );

        CheckCloseTag( "li", ref idx, "WARNING2: No se encontro el final del item de traducción" );
        }

      CheckCloseTag( "ol", ref idx, "WARNING2: No se encontro el final de la lista de traducción" );

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de un bloque 'hom'</summary>
    private bool ParseTrdInfoByBlockHom( ref int idx )
      {
      if( !Tags[idx].Txt.EndsWith( " class=\"hom\">" ) )  return false;

      ++idx;
      ParseNotaOrtografica( ref idx );

      string tp, grmTipo = null;
        
      SkipIsVacio( ref idx);
      while(  (tp=GetH3GrmTipo( ref idx )) != null )
        {
        if( tp!= null )
          {
          if( grmTipo != null )
            setMsg( "WARNING: El tipo gramatical '" + grmTipo + "' fue ignorado" );

          grmTipo = tp;
          }
        }

      if( grmTipo!=null )
        {
        nowGrmTipo = new GramarGroup( grmTipo );
        nowEntryData.GrmGrps.Add( nowGrmTipo );
        }
      else
        setMsg( "WARNING: No tiene tipo gramatical" );

      if( Tags[idx].Txt == "</div>" )
        {
        setMsg( "WARNING: Solo hay un tipo gramatical dentro de hom" );
        ++idx;
        return true;
        }

      while( ParseInflexiones( ref idx )>0   ||                           // Salta Inflexiones
             ParseSemantica( ref idx )       ||                           // Salta informacion Semantica
             ParseNotaOrtografica( ref idx)  ||                           // Salta información Ortografica
             SkipGramGrp( ref idx ).Length>0 ||                           // Salta información gramatical
             SkipSeparatorSimple( ref idx ) ) ;                           // Salta separador (, : br)

      var info = SkipParentisis( ref idx );                               // Obtiene información entre parentisis (Si hay)
      var Ref1 = GetReferencias( ref idx );
      var Lst  = ParseTrdListLavel1( ref idx );
      var Ref2 = GetReferencias( ref idx );

      if( Lst==false && Ref1==null && Ref2==null  && info==null)
        setMsg( "WARNING: No hay lista de sifnificados, ni referencias" );

      if( !CheckStartTag( "</div>", ref idx, "El final de la sección 'hom'" ) ) return false;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de un bloque 'hom' cuando esta dentro de un bloque 'dictionary biling'</summary>
    private bool ParseTrdInfoByBlockHom2( ref int idx )
      {
      if( Tags[idx].Txt != "<div class=\"hom\">" ) return false;
      ++idx;

      SkipIsVacio( ref idx );
      FormInfo( ref idx, out string info);
      GetReferecFrase( ref idx );

      string tp = GetSpanGrmTipo( ref idx );
      if( tp==null )
        setMsg( "WARNING: No tiene tipo gramatical" );
      else
        {
        nowGrmTipo = new GramarGroup( tp );
        nowEntryData.GrmGrps.Add( nowGrmTipo );
        }
                                
      if( Tags[idx].Txt == "</div>" )
        {
        ++idx;
        var Ref = GetReferencias( ref idx );
        if( Ref==null )
          setMsg( "WARNING: Solo hay un tipo gramatical dentro de hom" );

        return true;
        }

      string infoType = GetInitInfo( ref idx );

      for(;;)
        {
        bool ret = ParseSense( ref idx );
        bool der = ParseDerivadas( ref idx );
        bool frs = PaseFrasesEjemAndRefer( ref idx );
        var inflx = ParseInflexiones( ref idx );

        if( !ret && !der && !frs && inflx==0 ) break;
        }

      if( !CheckStartTag( "</div>", ref idx, "El final de la sección 'Hom2'" ) ) return false;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza una o mas palabras derivadas de la entrada actual</summary>
    private bool ParseDerivadas( ref int idx )
      {
      if( Tags[idx].Txt != "<div class=\"div content derivs\">" ) return false;

      SkipTag( "div", ref idx );
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de un bloque 'sense'</summary>
    private bool ParseSense( ref int idx )
      {
      if( Tags[idx].Txt != "<div class=\"sense\">" ) return false;
      ++idx;

      SkipBlocks( "<span class=\"span sensenum bluebold\">", ref idx );             // Salta número de la traducción
      SkipSeparator( ref idx );

      var     inflx = ParseInflexiones( ref idx );
      var infoSense = GetInitInfo( ref idx );

      int nMeans = 0;
      while( true )
        {
        SkipSeparator( ref idx );

        var sen = ParseSense( ref idx );
        var trd = GetTranslation( ref idx );
        var inf = PaseFrasesEjemAndRefer( ref idx );
        var txt = ExpandInfo( ref idx, out string info );
        var der = ParseDerivadas( ref idx );
           
        if( !sen && !trd && !inf && !txt && !der ) break;

        ++nMeans;
        //if( Tags[idx].Name == "br" ) ++idx;
        //if( Tags[idx].Txt.Trim()=="," ) ++idx;
        //if( Tags[idx].Txt.Trim()==":" ) ++idx;

        SkipBlocks( "<span class=\"span bluebold\">", ref idx );
        infoSense = GetInitInfo( ref idx );
        }

      if( nMeans == 0 )
        setMsg( "WARNING2: No se encontro información útil, en el grupo 'sense'" );

      CheckCloseTag( "div", ref idx, "++WARNING2: el final del bloque 'sense'" );

      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta un separador , o ;</summary>
    private void SkipSeparator( ref int idx )
      {
      if( Tags[idx].Txt != "<span class=\"span punctuation\">" ) return;
        
      var txt = Tags[idx+1].Txt.Trim();

      if( txt!=";" && txt!="," && txt!="≈" && txt!="=" && txt!="; =" ) return;

      if( Tags[idx+2].Txt == "</span>" ) idx +=3;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene información que precese una traducción dentro de un bloque 'sense'</summary>
    private string GetInitInfo( ref int idx )
      {
      string info = "";
      for(;;)
        {
        if( LabelTypeInfo( ref idx, out string sLabel ) )
          info += sLabel;
        else if( ExpandInfo( ref idx, out string sExpand ) )
          info += sExpand;
        else if( Tags[idx].Txt == "<span class=\"gramGrp\">" )
          info += GetTextTags( ref idx );
        else if( FormInfo( ref idx, out string sForm ) )
          info += sForm;
        else if( ParentisisInfo( ref idx, out string sInfo) )
          info += sInfo;
        else return info;
        }
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene información dentro de los bloques 'form type-....'</summary>
    private bool ExpandInfo( ref int i, out string info )
      {
      info = "";
      int idx = i;
      if( Tags[idx].Txt == "<span class=\"span punctuation\">" )
        {
        var txt = Tags[idx+1].Txt.Trim();
        if( (txt=="≈" || txt=="=" || txt=="; =") && Tags[idx+2].Txt == "</span>" )
          {
          idx +=3;
          info += txt + ' ';
          }
        }

      if( Tags[idx].Txt.IndexOf( "<span class=\"form type-" ) ==-1 ) return false;

      info += GetTextTags( ref idx );
      i = idx;
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene información dentro de los bloques 'lbl type-....'</summary>
    private bool LabelTypeInfo( ref int idx, out string info )
      {
      info = "";
      if( Tags[idx].Txt.IndexOf( "<span class=\"lbl type-" ) ==-1 ) return false;

      info = GetTextTags( ref idx );
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene información dentro de dos bloques 'form'</summary>
    private bool FormInfo( ref int idx, out string info )
      {
      info = "";
      if( Tags[idx].Txt != "<span class=\"form\">" )  return false;
      
      info = GetTextTags( ref idx );
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene información dentro de dos bloques 'punctuation'</summary>
    private bool ParentisisInfo( ref int i, out string info )
      {
      info = "";
      if( Tags[i].Txt != "<span class=\"span punctuation\">" )  return false;

      int idx = i+1;
      var txt = SkipParentisis( ref idx);
      if( txt== null || Tags[idx].Txt != "</span>" ) 
        return false;

      info = '(' + txt + ')';
      i = idx+1;
      return true;
      }

    //----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza las notas octograficas</summary>
    private bool ParseNotaOrtografica( ref int idx )
      {
      bool ret = false;
      while( Tags[idx].Txt == "<span class=\"orth\">" )                   // Nota de orografia
        {
        SkipTag( "span", ref idx );                                       // La salta
        ret = true;
        }

      return ret;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza el contenido de las referencias si existe</summary>
    private string GetReferencias( ref int idx )
      {
      string sRef = null;
      while( Tags[idx].Txt.IndexOf( "class=\"xr\"" )>=0 )              // Hace una referencia directa
        {
        SkipTag( Tags[idx].Name, ref idx );
        sRef += "Ref: ";

        if( Tags[idx].IsTxt && Tags[idx].Txt.Trim()=="," ) ++idx;
        }

      return sRef;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Trata de analizar las traducciones dentro de un bloque 'dictionary biling'</summary>
    private bool ParseTrdInfoByBlockBiling( ref int idx )
      {
      if( Tags[idx].Txt != "<div class=\"content definitions dictionary biling\">" ) return false;
      ++idx;

      var inflx = ParseInflexiones( ref idx );
      string infoEntry = GetInitInfo( ref idx );

      int nTrd = 0;                                                       // Trata de obtener las traduciones, por los formatos posibles
      while( ParseTrdInfoByBlockHom2( ref idx ) ||
             PaseFrasesEjemAndRefer( ref idx )   )
        {
        ++nTrd;
        if( Tags[idx].Name == "br" ) ++idx;
        if( Tags[idx].Txt.Trim()=="," ) ++idx;
        if( Tags[idx].Txt.Trim()==":" ) ++idx;
        }

      if( nTrd==0 )
        {
        setMsg( "No se puedo obtener ninguna traducción." );
        return false;
        }

      SkipBlocks( "<div class=\"div copyright\">", ref idx );
      SkipBlocks( "class='am-default contentslot'", ref idx );

      if( !CheckStartTag( "</div>", ref idx, "El final de la sección 'dictionary biling'" ) ) return false;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta todos los tags consecutivos que tengan el texto 'sTag'</summary>
    private bool SkipBlocks( string sTag, ref int i )
      {
      int idx = i;
      bool found = false;
      for( ; idx+1<Tags.Count; )
        {
        var tag = Tags[idx];
        if( tag.Txt.IndexOf(sTag)==-1 ) break;

        if( SkipTag( tag.Name, ref idx ) )
          i=idx;

        if( Tags[idx].Txt[0] == ',' ) ++idx;
        found = true;
        }

      return found;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Salta la informacion de redes sociales a partir del tag 'idx', si la hay</summary>
    private void SkipSocialButtons( ref int idx )
      {
      if( Tags[idx].Txt == "<div class=\"socialButtons\">" )              // Si hay informacion de redes sociales
        SkipTag( "div", ref idx );                                        // Salta sección de redes sociales
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza si hay información de sonido a partir del tag 'idx'</summary>
    private void ParseSonido( ref int idx )
      {
      var tag = Tags[idx];
      if( tag.Name=="span" && tag.Txt.IndexOf( "class=\"hwd_sound\"" ) < 0 ) return;

      SkipTag( "span", ref idx );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza si hay frases a partir del tag 'idx', 'pos' indica la posición donde esta ubicada la frase, 1- Después de la llave
    /// 2- Después del tipo gramatical y 3- Al final después de la definición de todos los tipos</summary>
    private int ParseFrases( ref int idx, int pos )
      {
      int nFrases = 0;

      for(; idx<Tags.Count; )
        {
        var tag = Tags[idx];
        if( tag.Name=="div" && tag.Txt.IndexOf( "class=\"p phrase\"" ) >= 0 )
          {
          // string s = MakeTagPartron( idx );
          // setMsg( s );

          SkipTag( "div", ref idx );

          if( pos==3 )
            {
            int tmp = idx;
            if( SkipTag( "h3", ref tmp ) && SkipTag( "ol", ref tmp ) )
              idx = tmp;
            }
          }
        else if( tag.Name=="p" && tag.Txt.IndexOf( "class=\"phrase\"" ) >= 0 )
          {
          SkipTag( "p", ref idx );
          }
        else if( !GetFraseFromPhr( ref idx) && !GetReferecFrase(ref idx) )
          break;

        ++nFrases;
        }

      return nFrases;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene una referencia a una frase </summary>
    private bool GetReferecFrase( ref int idx )
      {
      if( Tags[idx].Txt != "<div class=\"re type-phr\">" ) return false;

      SkipTag( "div", ref idx );
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el contenido de una frase a partir de un tag class="phr" </summary>
    private bool GetFraseFromPhr( ref int idx )
      {
      var found = false;
      var sFrase = "";
      while( Tags[idx].Txt == "<span class=\"phr\">" )
        {
        sFrase += GetTextTags( ref idx);
        found = true;

        if( idx+2 < Tags.Count && Tags[idx].Name=="i" && Tags[idx+1].IsTxt && Tags[idx+2].Name=="i" )
          {
          sFrase += " <" + Tags[idx+1].Txt + "> ";
          idx +=3;
          }
        else break;
        }

      if( !found ) return false;
        
      var frTrd = "";
      for(;;)
        {
        var i = idx;
        var    info = SkipParentisis( ref i );
        var GrmInfo = GetSpanGrmTipo( ref i );

        var sTrd = ParseTypeTranslation( ref i);
        if( sTrd==null )  break;

        frTrd += ", " + sTrd;

        if( Tags[i].Txt.Trim() == "," ) ++i;
        idx = i;
        }

      if( frTrd.Length==0 )
        setMsg( "------- Frase sin traducción ----------" );
        
      return found;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza todas las referencia a otras frases que alla</summary>
    private int ParseFrasesReference( ref int idx )
      {
      int nFrases = 0;

      for( ; idx+5<Tags.Count; )
        {
        if( Tags[idx  ].Txt.IndexOf( "class=\"p phrase\"" ) >= 0 &&
            Tags[idx+1].Txt.IndexOf( "class=\"xr\""       ) >= 0  )
          {
          SkipTag( Tags[idx].Name, ref idx );
          ++nFrases;
          }
        else break;
        }

      return nFrases;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa el encabezamiento de la sección de definición de las traducciones</summary>
    private bool ParseTrdsHeader( ref int idx )
      {
      idx = FindTag( "<div class=\"content-box-header\"", idx, 30 );      // Busca encabezamiento de informacion de traducción
      if( idx==-1 )                                                       // Si no lo encuentra 
        return setMsg( "No se encontro el encabezamiento de la seccion de traducciones" );

      ++idx;
      SkipBlocks( "<div class=\"word-frequency-container res_hos res_hot res_hod frenquency-title\">", ref idx );

      nowEntryKey = ParseKeyEntry( ref idx );
      if( nowEntryKey == null )
        return setMsg( "No se pudo encontrar el nombre de la entrada" );

      if( Tags[idx].Name=="span" && Tags[idx].Close==0 )                  // Variante que la pronunciacion con span
        {
        SkipTag( "span", ref idx );                                       // Salta la pronunciación
        }
      else
        {
        SkipTag( "div", ref idx );                                        // Salta la pronunciación
        SkipTag( "div", ref idx );                                        // Salta div vacio
        }

      ParseNotaOrtografica( ref idx );
      ParseInflexiones( ref idx );

      if( Tags[idx].Txt == "," ) ++idx;                                   // Salta la coma si la hay

      ParseNotaOrtografica( ref idx );

      if( !CheckStartTag( "</div>", ref idx ) )
        return setMsg( "No se termino de analizar el encabezamiento de la seccion de traducciones" ); 

      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza las inflexiones si las hay</summary>
    private int ParseInflexiones( ref int idx )
      {
      int count = 0;

      for( ; idx<Tags.Count; )
        {
        if( Tags[idx].Txt == "<div class=\"inflected_forms\">" )
          {
          SkipTag( "div", ref idx );
          ++count;
          }
        else if( Tags[idx].Txt == "<span class=\"form inflected_forms type-infl\">" )
          {
          SkipTag( "span", ref idx );
          ++count;
          }
        else break;
        }

      return count;
      }

    static char[] Nada = {' ', ',', ':'};
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el nombre del tipo gramatical</summary>
    private string GetH3GrmTipo( ref int idx )
      {
      string txt = ""; 
      if( Tags[idx].Txt != "<h3 class=\"gramGrp h3_entry\">" )            // Hay una definición de tipo gramatical
        {
        if( Tags[idx].Txt != "<h3 class=\"h3_entry\">" )            // Hay una definición de tipo gramatical
          return null;

        txt = "Ref: ";
        }

      txt += GetTagText( ref idx ).Trim(Nada);                           // Obtiene el texto dentro del tab H3
      if( txt.Length == 0 )                                               // Si el texto esta vacio
        {
        int tmp = idx;
        var txt2 = GetH3GrmTipo( ref tmp );                               // Trata de buscar dos consecutivos
        if( txt2 != null )                                                 // Si lo enconto
          {
          txt = txt2;                                                     // Toma el texto del segundo
          idx = tmp;                                                      // Actualiza el ultimo tag leido
          }
        }

      while( Tags[idx].Txt.StartsWith( "<a class=\"link-right verbtable\"" ) )
        {
        SkipTag( "a", ref idx );                                    
        if( string.IsNullOrEmpty(txt) )
          txt = "Verb";
        }

      return txt;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el nombre del tipo gramatical</summary>
    private string GetSpanGrmTipo( ref int idx )
      {
      string txt = "";
      if( Tags[idx].Txt != "<span class=\"gramGrp\">" ) 
        return GetH3GrmTipo( ref idx );                                   // No hay una definición de tipo gramatical

      txt += GetTagText( ref idx ).Trim( Nada );                          // Obtiene el texto dentro del tab span
      if( txt.Length == 0 )                                               // Si el texto esta vacio
        {
        setMsg( "WARNING: Tipo gramatical vacio" );
        }

      return txt;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene el texto dentro del tag actual</summary>
    private string GetTagText( ref int Idx )
      {
      string txt = "";

      int idx = Idx;
      if( idx>=Tags.Count    ) return txt;
      if( Tags[idx].Close!=0 ) return txt;
      if( Tags[idx].IsTxt    ) return Tags[Idx++].Txt;

      var Name = Tags[idx].Name;

      ++idx;
      int level = 1;
      for( ; idx<Tags.Count; idx++ )
        {
        var tag = Tags[idx];
        if( tag.Name == Name )
          {
          if( tag.Close==0 ) ++level;
          if( tag.Close==1 ) --level;
          }

        if( tag.IsTxt )
          {
          if( txt.Length>0 && !txt.EndsWith(" ") ) txt += ' ';

          txt += tag.Txt;
          }

        if( level==0 ) { Idx=idx+1; return txt; };
        }

      return txt;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene todos los tags que representan un texto que esten consecutivos</summary>
    private string GetTextTags( ref int Idx )
      {
      string txt = "";

      int idx = Idx;
      if( idx>=Tags.Count ) return txt;
      if( Tags[idx].Close!=0 ) return txt;
      if( Tags[idx].IsTxt ) return Tags[Idx++].Txt;

      var Name = Tags[idx].Name;

      ++idx;
      int level = 1;
      for( ; idx<Tags.Count; idx++ )
        {
        var tag = Tags[idx];
        if( tag.Name == Name )
          {
          if( tag.Close==0 ) ++level;
          if( tag.Close==1 ) --level;
          }

        if( level==0 ) { Idx=idx+1; return txt; };

        if( tag.IsTxt )
          {
          if( txt.Length>0 && !txt.EndsWith( " " ) ) txt += ' ';

          txt += tag.Txt;
          }
        else if( idx+2 < Tags.Count && (Tags[idx].Name=="em" && Tags[idx+1].IsTxt && Tags[idx+2].Name=="em" ||
                                        Tags[idx].Txt =="<span class=\"lbl type-pos\">" && Tags[idx+1].IsTxt && Tags[idx+2].Name=="span" ) )
          {
          switch( Tags[idx+1].Txt )
            {
            case "adj m":
            case "m":               txt += " m.";          break;
            case "adj f":           
            case "f":               txt += " f.";          break;
            case "plural":
            case "pluriel":
            case "(pl)":            
            case "pl":              txt += " pl.";         break;
            case "pl inv":          txt += " pl. inv.";    break;
            case "fpl":             txt += " f. pl.";      break;
            case "mpl":             txt += " m. pl.";      break;
            case "m or f":          
            case "m/f":             
            case "mf":              txt += " m. f.";       break;
            case "m sing":          txt += " m. s.";       break;
            case "f sing":          txt += " f. s.";       break;
            case "inv":             txt += " inv.";        break;
            case "m inv":           txt += " m. inv.";     break;
            case "mpl inv":         txt += " m. f. inv.";  break;
            case "f inv":           txt += " f. inv.";     break;
            case "m inv or f inv": 
            case "mf inv":          txt += " m. f. inv.";  break;
            case "inv adj":         txt += " adj.inv.";    break;
            case "sg":
            case "sing":            txt += " s.";          break;
            case "etc":             txt += " etc.";        break;
            case "m sing inv":      txt += " m. s. inv";   break;
            case "mpl/fpl":
            case "mpl or fpl":
            case "mfpl":            txt += " m. f. pl.";   break;
            case "o":
            case "or":              txt += "|";             break;
            case "adj":                                     break;
            case "modif":
            case "nmf":
              break;
            default:
              setMsg( "Significado desconocido de '"+ Tags[idx+1].Txt +"' en el significado" ); break;
            }

          idx += 2;
          if( Tags[idx].Name == Name && Tags[idx].Close==1 )--level;
          }
        else if( idx+2 < Tags.Count && Tags[idx].Name=="sup" && Tags[idx+1].IsTxt && Tags[idx+2].Name=="sup" )
          {
          txt += '^' + Tags[idx+1].Txt;
          idx += 2;
          }
        else if( tag.Name != "span" && tag.Name != "q" && tag.Name != "strong" && tag.Name != "a" )
          { Idx=idx; return txt; };
        }

      return txt;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private string MakeTagPartron( int idx )
      {
      string txt = "";

      if( idx>=Tags.Count ) return txt;
      if( Tags[idx].Close!=0 ) return txt;
      if( Tags[idx].IsTxt ) return Tags[idx++].Txt;

      var Name = Tags[idx].Name;

      ++idx;
      int level = 1;
      for( ; idx<Tags.Count; idx++ )
        {
        var tag = Tags[idx];
        if( tag.Name == Name )
          {
          if( tag.Close==0 ) ++level;
          if( tag.Close==1 ) --level;
          }

        if( level==0 ) return txt;

        if( tag.IsTxt ) txt += "text";
        else            txt += tag.Txt;
        }

      return txt;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza el contenido de la seccion de semántica</summary>
    private bool ParseSemantica( ref int idx )
      {
      bool ret = false;
      for( ; idx<Tags.Count; )
        {
        if( Tags[idx].Txt != "<div class=\"semantic\">" && 
            Tags[idx].Txt != "<div class=\"lbl semantic\">" ) break;

        SkipTag( "div", ref idx );
        ret = true;
        }

      return ret;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un texo que este entre parentisi</summary>
    private string SkipParentisis( ref int idx )
      {
      string Text = "";
      bool End = false;

      while( idx<Tags.Count && !End )
        {
        int i = idx;
        var tag = Tags[i++];

        if( tag.IsTag || tag.Txt[0] != '(' ) break;

        if( Text.Length>0    ) Text += "; ";
        if( tag.Txt.Length>1 ) Text += tag.Txt.Substring(1);

        int level = 1;
        for( ; i<Tags.Count; ++i )
          {
          tag = Tags[i];
          var Txt = tag.Txt;

          if( tag.IsTxt )
            {
            int k=0;
            for( ;k<Txt.Length; ++k)
              {
              if( Txt[k]=='(' ) ++level;
              if( Txt[k]==')' ) --level;

              if( level==0 ) break;
              }

            if( level==0 )
              {
              idx=i;
              if( k >= Txt.Length-1 ) ++idx;
              if( k >0              ) Text += Txt.Substring(0,k-1);
              if( k < Txt.Length-1  ) Tags[i].Txt = Txt.Substring( k+1 ).TrimStart();

              break; 
              }
            }

          if( tag.IsTxt ) Text += ' ' + Txt;
          else if( tag.Name=="span" || tag.Name=="strong" || tag.Name=="i"  || tag.Name=="q" || tag.Name=="em" ) continue;
          else { End=true; break; }
          }
        }

      if( Text.Length==0 || End) return null;

      if( Tags[idx+0].Txt.StartsWith(",")                  && 
          Tags[idx+1].Name=="span" && Tags[idx+1].Close==0 &&
          Tags[idx+2].IsTxt        &&
          Tags[idx+3].Name=="span" && Tags[idx+3].Close==1 &&
          Tags[idx+4].Txt.StartsWith( ")" )
          )
        {
        Text += ", " + Tags[idx+2].Txt;
        idx += 5;
        }

      return Text.Replace( "&nbsp;", " " ).Replace( "  ", " " ).Trim();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Procesa los tags entre 'iIni' e 'iEnd' para obtener el nombre de la entrada</summary>
    private string ParseKeyEntry( ref int idx )
      {
      if( Tags[idx].Txt != "<h2 class=\"h2_entry\">" ) return null;       // No esta posicionado al inicio de la definicion

      int iIni = idx+1;
      if( !SkipTag( "h2", ref idx ) ) return null;                        // Va hasta el final del nombre de la entrada
      int iEnd = idx-1;

      string name = "";
      for( ; iIni<iEnd; ++iIni )
        {
        var tag = Tags[iIni];

        if( tag.IsTxt ) name += ' ' + tag.Txt;
        else if( iIni+2 < iEnd && Tags[iIni].Name=="em" && Tags[iIni+2].Name=="em" )
          {
          switch( Tags[iIni+1].Txt )
            {
            case "or":
            case "o a veces":
            case "o": name += '|'; break;
            case "etc":  break;
            default:
              setMsg( "Significado desconocido de '"+ Tags[iIni+1].Txt +"' en el nombre de la entrada" );
              return null;
            }

          iIni += 2;
          }
        else if( tag.Name=="span" || tag.Name=="strong" )
          {
          if( tag.Txt == "<span class=\"homnum\">" ) name += '_';
          continue;
          }
        else if( tag.Name=="sup" )
          {
          if( tag.Close==0 ) name += '^';
          continue;
          }
        else break;
        }

      if( iIni != iEnd ) setMsg("Quedo información el nombre de la entrada por analizar");

      name = name.Replace( "  ", " ").Replace( "\"", "" ).Trim( Cascara );
      if( name.Length == 0 )  return null;

      return name;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Chequea que el tag en 'idx' comience con el 'txt' si es positivo, lo salta y retorna 'true', en otro caso retorna
    /// 'false' y opcionalmente muestra el mensaje 'Msg'</summary>
    private bool CheckStartTag( string txt, ref int idx, string Msg=null )
      {
      if( !Tags[idx].Txt.StartsWith(txt) ) 
        {
        if( Msg!=null )
          Msgs?.Invoke( "No se encontro " + Msg + " --> "+ fileName );

        return false;
        }

      ++idx;
      return true;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Toma el tag actual, que dede ser un tag del tipo 'sTab', y salta hasta el final, si no se puede realizar la operación
    /// opcionalmente coloca un mensaje y retorna falso</summary>
    private bool SkipTag( string sTag, ref int idx, string Msg=null )
      {
      int i = idx;
      if( i>=Tags.Count ) return setMsg( Msg );

      var tag = Tags[i];
      if( tag.Name!=sTag ) return setMsg( Msg );
      if( tag.Close==1   ) return setMsg( Msg );

      ++i;
      if( tag.Close==2 ) { idx=i; return true; }

      int level = 1;
      for( ; i<Tags.Count; i++ )
        {
        tag = Tags[i];
        if( tag.Name != sTag ) continue;

        if( tag.Close==0 ) ++level;
        if( tag.Close==1 ) --level;

        if( level==0 ) { idx=i+1; return true; };
        }

      return setMsg( Msg );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone el mensaje 'msg' si es diferente de null y retorna false</summary>
    private bool setMsg( string msg )
      {
      if( msg!=null && !msg.StartsWith( "WARNING" ) )
        Msgs?.Invoke( msg + " --> "+ fileName );

      return false;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca al tag con el texto indicado y retorna su indice, o -1 si no lo encuentra</summary>
    private int FindTag( string txt, int idx, int max=-1 )
      {
      max = max<=0? Tags.Count : idx+max;

      for( int i= idx; i<max; i++ )
        {
        var tag = Tags[i];
        if( tag.Txt.IndexOf(txt) >= 0 ) return i;
        }

      return -1;
      }
    }
  }
