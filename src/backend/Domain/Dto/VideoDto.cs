using System.Collections.Generic;

namespace YTUploader.Dto
{

    public enum EPrivacidade
    {
        NaoListado,
        Publico,
        Privado
    }

    public class VideoDto
    {
        public VideoDto()
        {
            Tags = new List<string>();
            Privacidade = EPrivacidade.NaoListado;
        }

        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Categoria { get; set; }        
        public byte[] Conteudo { get; set; }
        public List<string> Tags{get;set;}

        public EPrivacidade Privacidade {get;set;}
    }
}