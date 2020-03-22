namespace YTUploader.Dto
{
    public class VideoDto
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Categoria { get; set; }        
        public byte[] Conteudo { get; set; }
    }
}