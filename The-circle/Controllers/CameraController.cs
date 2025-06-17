using Microsoft.AspNetCore.Mvc;
    using The_circle.Application;
    
    namespace The_circle.Controllers;
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserCameraController : ControllerBase
    {
        private readonly IUserCameraReadRepository _readRepository;
    
        public UserCameraController(IUserCameraReadRepository readRepository)
        {
            _readRepository = readRepository;
        }
    
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCameraState(Guid userId)
        {
            var userCamera = await _readRepository.GetByUserIdAsync(userId);
            return Ok(new { isOn = userCamera?.IsOn ?? false });
        }
    }