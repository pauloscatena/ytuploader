using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YTUploader.Dto;

namespace YTUploader.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploaderController: ControllerBase
    {
        [HttpPost]
        public async Task Upload(VideoDto dados)
        {
            var uploader = new YTUploader.YTEnvio();
            await uploader.Upload(dados);
        }
    }
}