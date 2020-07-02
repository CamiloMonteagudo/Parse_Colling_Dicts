using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParseCollingDicts.Properties;

namespace ParseCollingDicts
  {
  public partial class Form1 : Form
    {
    string[] files;
    int nowFile;
    bool Pause;

    Dictionary<string, EntryData> Entries = new Dictionary<string, EntryData>();

    public Form1()
      {
      InitializeComponent();
      nowFile    = 0;
      EnableSave( false );

      txtPagesDir.Text = Settings.Default.WebsDir;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private void btnPathIni_Click( object sender, EventArgs e )
      {
      SelFolderDlg.SelectedPath = txtPagesDir.Text;
      SelFolderDlg.Description = "Seleccione el directorio a leer";

      if( SelFolderDlg.ShowDialog() == DialogResult.OK )
        txtPagesDir.Text = SelFolderDlg.SelectedPath;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private void btnLoad_Click( object sender, EventArgs e )
      {
      try
        {
        switch( btnParse.Text )
          {
          case "Analizar":
            if( tabParse.SelectedIndex == 0 ) IniParseDir();
            else                              ParseOnePages();
            break;

          case "Pausar":
            Pause = true;
            btnParse.Text ="Continuar";
            btnCancel.Visible = true;
            EnableSave( true );
            break;

          case "Continuar":
            ContinueParseDir();
            break;
          }
        }
      catch( Exception exc )
        {
        MessageBox.Show( "Error:" + exc.Message );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private void ParseOnePages()
      {
      txtMsgBox.Text = "";
      EnableSave( false );
      lbNowPage.Text = "";

      Dictionary<string, EntryData> dict = new Dictionary<string, EntryData>();

      var path = Path.Combine( txtPagesDir.Text, txtPageName.Text);

      ParsePage( path, dict );

      var txt = DictToText( dict, "\r\n\r\n");

      SetMessage( txt );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private void IniParseDir()
      {
      Settings.Default.WebsDir = txtPagesDir.Text;
      Settings.Default.Save();

      files   = Directory.GetFiles( txtPagesDir.Text );
      nowFile = 0;

      txtMsgBox.Text = "";
      Entries = new Dictionary<string, EntryData>();

      ContinueParseDir();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    private void ContinueParseDir()
      {
      Pause = false;
      btnParse.Text = "Pausar";
      btnCancel.Visible = false;
      EnableSave( false );

      ParsePages();

      if( nowFile >= files.Length )
        {
        btnParse.Text = "Analizar";
        EnableSave( true );

        SetMessage( "\r\n ***** ANALIZADAS TODAS LA PÁGINAS ("+ nowFile +") *****" );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Atiende el boton para cancelar el analisis</summary>
    private void btnCancel_Click( object sender, EventArgs e )
      {
      Pause = false;
      btnParse.Text = "Analizar";
      btnCancel.Visible = false;
      EnableSave( true );

      SetMessage( "\r\nANALIZADAS "+ nowFile +" PÁGINAS ");
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Busca todas las páginas WEB y las analiza una por una</summary>
    public void ParsePages()
      {
      for( ; nowFile<files.Length; nowFile++ )
        {
        Application.DoEvents();
        if( Pause ) return;

        lbNowPage.Text = "Analizando página " + nowFile + " de " + files.Length;

        ParsePage( files[nowFile], Entries );
        }
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Analiza el contenido de una pagina y pone su contenido en el diccionario actual</summary>
    public void ParsePage( string pgName, Dictionary<string, EntryData> dict )
      {
      var page = new CollingWordPage( pgName, SetMessage );

      page.ParsePageSeccions( dict );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Pone un mensaje para información del usuario</summary>
    private void SetMessage( string msg )
      {
      txtMsgBox.Text += (msg + "\r\n");
      txtMsgBox.Focus();
      txtMsgBox.SelectionStart = txtMsgBox.Text.Length + 1;
      txtMsgBox.ScrollToCaret();
      Application.DoEvents();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Expresa el contenido del diccionario como una cadena de texto</summary>
    private string DictToText( Dictionary<string, EntryData> dict, string WordSep = "\r\n" )
      {
      var s = new StringBuilder( 200*dict.Count );

      foreach( var item in dict )
        {
        var key = item.Key;
        var Data = item.Value;

        var sData = Data.ToString();
        if( sData.Trim().Length>0 )
          {
          s.Append( "-->" );
          s.Append( key.TrimEnd() );
          s.Append( sData );
          s.Append( WordSep );
          }
        }

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene la información sobre todos los tipos gramaticales presentes en el diccionario</summary>
    private string GetGramTipos( Dictionary<string, EntryData> dict )
      {
      var infoTipos = new Dictionary<string,int>();

      foreach( var item in dict )
        {
        var key = item.Key;
        var Data = item.Value;

        if( key.Contains( ' ' ) ) continue;

        foreach( var Grm in Data.GrmGrps )
          {
          if( Grm.Means.Count==0 ) continue;

          var Names = Grm.Name.Split(',');
          foreach( var Name in Names )
            {
            var tName = Name.Trim();
            if( tName.Length==0 ) continue;

            if( infoTipos.ContainsKey(tName) ) infoTipos[tName] += 1;
            else                               infoTipos.Add( tName, 1);
            }
          }
        }

      var s = new StringBuilder( 40*infoTipos.Count );

      foreach( var item in infoTipos )
        {
        s.Append( item.Key );
        s.Append( " --> " );
        s.Append( item.Value );
        s.Append( "\r\n" );
        }

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene las entradas del dicionario 'dic' que tengan uno de los tipos listado en 'filter'</summary>
    private string GetTipos( Dictionary<string, EntryData> dict, HashSet<string> filter=null)
      {
      var s = new StringBuilder( (int)(80*dict.Count*0.3) );

      foreach( var item in dict )
        {
        var key = item.Key;
        var Data = item.Value;

        if( key.Contains( ' ' ) ) continue;

        var fMeans = new GramarGroup("");
        foreach( var Grm in Data.GrmGrps )
          {
          if( Grm.Means.Count==0 ) continue;

          var Names = Grm.Name.Split(',');
          foreach( var Name in Names )
            {
            var tName = Name.Trim();

            if( tName.Length==0 ) continue;
            if( filter!=null && !filter.Contains(tName) ) continue;

            foreach( var mean in Grm.Means )
              {
              string strMean = mean.sMean.RemoveEntre('(',')');
              if( strMean.StartsWith("to ") )
                strMean = strMean.Substring(3);

              strMean = strMean.Trim();
              if( !strMean.Contains(' ') ) 
                fMeans.AddIfNoExist( new Mean(strMean) );
              }
            }
          }

        var sMean = "";
        foreach( var mean in fMeans.Means )
          {
          if( sMean.Length>0 ) sMean += ", ";
          sMean += mean.sMean;
          }

        if( sMean.Length>0 )
          {
          s.Append( key );
          s.Append( '\\' );
          s.Append( sMean );
          s.Append( "\r\n" );
          }
        }

      return s.ToString();
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    static HashSet<string> FiltterVerbs = new  HashSet<string> { "Verb",
                                                                 "auxiliary verb",
                                                                 "copulative verb",
                                                                 "impersonal verb",
                                                                 "intransitive reflexive verb",
                                                                 "intransitive verb",
                                                                 "modal verb",
                                                                 "passive reflexive verb",
                                                                 "reciprocal reflexive verb",
                                                                 "reflexive verb",
                                                                 "separable transitive verb",
                                                                 "transitive reflexive verb",
                                                                 "transitive verb",
                                                                 "transitive verb + adverb",
                                                                 "verb",
                                                                 "verbo",
                                                                 "verbo auxiliar",
                                                                 "verbo intransitivo",
                                                                 "verbo intransitivo or verbo transitivo",
                                                                 "verbo modal",
                                                                 "verbo reflexivo",
                                                                 "verbo transitivo",
                                                                 "verbo transitivo or sustantivo",
                                                                 "verbo transitivo or verbo intransitivo"};
    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Guarda toda la información analizada a un fichero</summary>
    private void btnSave_Click( object sender, EventArgs e )
      {
      //var txt = DictToText( Entries );

      //var txt = GetGramTipos( Entries );
      //txtSaveName.Text = "../TiposInfo.txt";

      var txt = GetTipos( Entries, FiltterVerbs );
      txtSaveName.Text = "../Verbos.txt";

      var path = Path.Combine( txtPagesDir.Text, txtSaveName.Text);

      File.WriteAllText( path, txt );

      SetMessage( "\r\nDATOS GUARDADOS EN:\r\n"+ path );
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Abilita o desavilita la opcion de guardar el diccionario en un fichero</summary>
    private void EnableSave( bool save )
      {
      lbSave.Visible = save;
      txtSaveName.Visible = save;
      btnSave.Visible = save;
      }

    ///-----------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Localiza la página que se quiere localizar</summary>
    private void btnSelPage_Click( object sender, EventArgs e )
      {
      SelFileDlg.Title  = "Seleccione Página Web";
      SelFileDlg.Filter = "Página Web que defina la entrada (*.html)|*.html";
      SelFileDlg.InitialDirectory = txtPagesDir.Text;

      if( SelFileDlg.ShowDialog( this ) == DialogResult.OK )
        {
        var path = SelFileDlg.FileName;

        txtPagesDir.Text = Path.GetDirectoryName(path);
        txtPageName.Text = Path.GetFileName(path)  ;
        }
      }
    }
  }
