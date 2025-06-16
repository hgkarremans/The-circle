using Microsoft.AspNetCore.Mvc;
    using The_circle.Application;
    
    namespace The_circle.Controllers;
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserCameraController : ControllerBase
    {
        private readonly IUserCameraReadRepository _readRepository;
        private readonly ToggleCameraCommandHandler _toggleCameraHandler;
    
        public UserCameraController(IUserCameraReadRepository readRepository, ToggleCameraCommandHandler toggleCameraHandler)
        {
            _readRepository = readRepository;
            _toggleCameraHandler = toggleCameraHandler;
        }
    
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCameraState(Guid userId)
        {
            var userCamera = await _readRepository.GetByUserIdAsync(userId);
            return Ok(new { isOn = userCamera?.IsOn ?? false });
        }
    
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleCamera([FromBody] ToggleCameraCommand command)
        {
            await _toggleCameraHandler.Handle(command, default);
            return Ok();
        }
    }