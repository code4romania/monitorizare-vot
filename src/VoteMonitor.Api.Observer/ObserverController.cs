//using System.Threading.Tasks;
//using System.Linq;
//using System;
//using System.Net;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using MonitorizareVot.Api.ViewModels;
//using MonitorizareVot.Domain.Ong.ObserverAggregate;
//using MonitorizareVot.Ong.Api.Extensions;


//namespace MonitorizareVot.Api.Controllers
//{
//    [Route("api/v1/observer")]
//    public class ObserverController : Controller
//    {
//        private readonly IMediator _mediator;
//        private readonly ILogger _logger;
//        //private readonly IMapper _mapper;

//        public ObserverController(IMediator mediator, ILogger logger)
//        {
//            _mediator = mediator;
//            _logger = logger;
//        }

//        [Authorize]
//        [HttpPost]
//        public async Task<dynamic> NewObserver(ObserverModel model)
//        {
//            if (!ModelState.IsValid)
//                return BadRequest(ModelState);

//            await _mediator.Send(_mapper.Map<NewObserverRequest>(model));

//            return Task.FromResult(new { });
//        }

//        //[Authorize]
//        //[HttpPost]
//        //[Route("reset")]
//        //public async Task<IAsyncResult> Reset([FromForm] string action, [FromForm] string phoneNumber)
//        //{
//        //    if (String.IsNullOrEmpty(action) || String.IsNullOrEmpty(phoneNumber))
//        //        return Task.FromResult(BadRequest());

//        //    if (String.Equals(action, ControllerExtensions.DEVICE_RESET))
//        //    {
//        //        var result = await _mediator.Send(new ResetDeviceCommand(ControllerExtensions.GetIdOngOrDefault(this, 0), phoneNumber));
//        //        if (result == -1)
//        //            return Task.FromResult(NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + phoneNumber));
//        //        else
//        //            return Task.FromResult(Ok(result));
//        //    }

//        //    if (String.Equals(action, ControllerExtensions.PASSWORD_RESET))
//        //    {
//        //        var result = await _mediator.Send(new ResetPasswordCommand(ControllerExtensions.GetIdOngOrDefault(this, 0), phoneNumber));
//        //        if (String.IsNullOrEmpty(result))
//        //            return Task.FromResult(NotFound(ControllerExtensions.RESET_ERROR_MESSAGE + phoneNumber));
//        //        else
//        //            return Task.FromResult(Ok(result));
//        //    }

//        //    return Task.FromResult(UnprocessableEntity());
//        //}
//    }
//}
