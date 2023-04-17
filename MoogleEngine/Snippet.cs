namespace MoogleEngine;

/**
*Clase para computar el snippet.
*Un snippet es un trozo de documento. Un snippet es un subdocumento o subconjunto del documento.
**/

static class Snippet{
    private const int SnippetLength = 20;//Cantidad maxima de terminos mostrados en el snippet

    //Computa el mejor snippet para el documento basado en la consulta
    public static string GetSnippet(string query,Documento doc){
        string snippet = "";
        
        //Creo un Documento a partir de mi query
        Documento queryDoc = new Documento(query,"I am your query");
        
        //Determino los distintos terminos de mi query
        string[] terminosQuery = queryDoc.GetUniqueTerms();
        
        //Corpus de subDocumentos formados por los terminos de query
        List<Documento> subDocumentos = new List<Documento>();

        string[] terminosDocumento = doc.Terminos;//Terminos del documento
        //Por cada aparicion de algun termino de la query en el documento
        foreach(string term in terminosQuery){
            if(!Valorador.FrecuenciaBooleana(term,doc))continue;
            foreach(int pos in doc.Contenido[term]){

                //Construyo un minidocumento con esta seccion del documento
                string textoSubDocumento = "";
                for(int i = Math.Max(0,pos - SnippetLength / 2), snippetWords = 0;i < terminosDocumento.Length && snippetWords < SnippetLength;++i,++snippetWords){
                    textoSubDocumento += terminosDocumento[i] + " ";
                }
                subDocumentos.Add(new Documento(textoSubDocumento,"I am your subDocument"));
            }
        }

        //Si no aparece ningun termino en este documento
        if(subDocumentos.Count == 0)return snippet;

        double[] valor = Valorador.Valorar(terminosQuery,subDocumentos.ToArray());
        
        //Mi snippet es el subdocumento de mejor score
        int posicionMejor = 0;
        double valorMejor = double.MinValue;
        for(int i=0;i<valor.Length;++i){
            if(valorMejor < valor[i]){
                valorMejor = valor[i];
                posicionMejor = i;
            }
        }

        foreach(string s in subDocumentos[posicionMejor].Terminos)snippet += s + " ";

        return snippet;
    }
}