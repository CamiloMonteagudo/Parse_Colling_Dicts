namespace ParseCollingDicts
  {
  public class Mean
    {
    public string sMean;

    ///---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Crea un objeto con los datos de un significado</summary>
    public Mean( string sTrd ) => this.sMean=sTrd;

    ///---------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Retorna una representación del objeto en forma de cadena de caracteres</summary>
    public override string ToString()
      {
      return '"' + sMean + '"';
      }
    }
  }