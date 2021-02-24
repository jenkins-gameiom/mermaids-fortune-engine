using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using AGS.Slots.MermaidsFortune.Platform;
using AGS.Slots.MermaidsFortune.WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Autofac.Features.Indexed;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Logic;
using Autofac;

namespace AGS.Slots.MermaidsFortune.WebAPI
{
    public class RequestManager
    {

        private readonly Game _game;
        private readonly IRequestContext _context;
        private readonly ILogger<RequestManager> _logger;
        private readonly IMathFileService _mathFileService;


        public static int SpinCounter = 0;
        public static int SessionCounter = 0;
        public static int InitGameCounter = 0;
        public static int ErrorRequests = 0;
        public static DateTime LastAction;

        public RequestManager(IMathFileService mathFileService, Game game, ILogger<RequestManager> logger, IRequestContext context)
        {
            _game = game;
            _logger = logger;
            _context = context;
            _mathFileService = mathFileService;


        }

        public  string HandleRequest(PlatformRequest platformRequest)
        {
            string result = null;
            dynamic response = null;
            try
            {
                dynamic request = null;
                _context.State = platformRequest.PrivateState;

                _context.RequestItems = new RequestItems
                {
                    betAmount = platformRequest.PublicState.betAmount.GetValueOrDefault(),
                    denom = platformRequest.PublicState.denom.GetValueOrDefault(),
                    isFreeSpin = platformRequest.PublicState.action == ActionType.freespin.ToString(),
                    action = platformRequest.PublicState.action,
                    force = platformRequest.PublicState.force,
                    cleanState = platformRequest.PublicState.cleanState
                };
                _context.Config = platformRequest.Config;
                ValidateRTP(_context.Config);
                if (_context.Config.rtp == 96.0)
                {
                    _context.MathFile = _mathFileService.GetMathFile(MathFileType.Config96);
                }
                if (_context.Config.rtp == 94.0)
                {
                    _context.MathFile = _mathFileService.GetMathFile(MathFileType.Config94);
                }
                if (_context.MathFile == null)
                    throw new Exception("RTP error occured");

                var dynamicRequest = Json.ObjectToDynamic(platformRequest);


                LastAction = DateTime.Now;
                if (platformRequest.PublicState.action == ActionType.init.ToString())
                {

                    try
                    {
                        //request = Commons.Json.Clone(dynamicRequest);
                        response = _game.InitSlot(dynamicRequest);
                        result = Json.Encode(response);
                        if (Json.HasProperty(response, "error"))
                        {
                            ErrorRequests++;// request, "MermaidsFortune", ActionType.init.ToString()
                            _logger.LogError(new Exception((string)response.error.message), (string)string.Format("{0} generated an error, the request is:{1},action typeis:{2}", request, "MermaidsFortune", ActionType.init));
                        }
                        InitGameCounter++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError((string)string.Format("MermaidsFortune generated an error, the request is:{0},action typeis:{1}", request, ActionType.init));
                        throw ex;
                    }
                }
                else if (platformRequest.PublicState.action == ActionType.spin.ToString() || platformRequest.PublicState.action == ActionType.freespin.ToString())
                {
                    try
                    {
                        //request = Commons.Json.Clone(dynamicRequest);
                        response =  _game.Spin(dynamicRequest);
                        response.privateState.lastState = response.publicState;
                        result = Json.Encode(response);
                        if (Json.HasProperty(response, "error"))
                        {
                            ErrorRequests++;
                            _logger.LogError((string)string.Format("MermaidsFortune generated an error during bonuspick, the request is:{0},action typeis:{1} error is:{2}", request, ActionType.spin, (string)response.error.stackTrace));
                        }
                        SpinCounter++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError((string)string.Format("MermaidsFortune generated an error during bonuspick, the request is:{0},action typeis:{1} stack trace is:{2}", request, ActionType.spin, (string)response.error.stackTrace));
                        throw ex;
                    }
                }
                else if (platformRequest.PublicState.action == ActionType.pick.ToString())
                {
                    try
                    {
                        //request = Commons.Json.Clone(dynamicRequest);
                        response =  _game.Pick(dynamicRequest);
                        response.privateState.lastState = response.publicState;
                        result = Json.Encode(response);
                        if (Json.HasProperty(response, "error"))
                        {
                            ErrorRequests++;
                            _logger.LogError((string)string.Format("MermaidsFortune generated an error during bonuspick, the request is:{0},action typeis:{1} stack trace is:{2}", request, ActionType.pick, (string)response.error.stackTrace));

                        }
                        SpinCounter++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, (string)string.Format("MermaidsFortune generated an error during bonuspick, the request is:{0},action typeis:{1} stack trace is:{2}", request, ActionType.pick, (string)response.error.stackTrace));
                        throw ex;
                    }
                }
                else if (platformRequest.PublicState.action == ActionType.jackpotpick.ToString())
                {
                    try
                    {
                        //request = Commons.Json.Clone(dynamicRequest);
                        response =  _game.JackpotPick(dynamicRequest);
                        response.privateState.lastState = response.publicState;
                        result = Json.Encode(response);
                        if (Json.HasProperty(response, "error"))
                        {
                            ErrorRequests++;
                            _logger.LogError((string)string.Format("MermaidsFortune generated an error during jackpotpick, the request is:{0},action typeis:{1} stack trace is:{2}", request, ActionType.jackpotpick, (string)response.error.stackTrace));

                        }
                        SpinCounter++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, (string)string.Format("MermaidsFortune generated an error during jackpotpick, the request is:{0},action typeis:{1} stack trace is:{2}", request, ActionType.jackpotpick, (string)response.error.stackTrace));
                        throw ex;
                    }
                }
                else
                {
                    var message = (string)string.Format("MermaidsFortune generated an error, unknown command during spin, the request is:{0}, stack trace is:{1}", dynamicRequest, (string)response.error.stackTrace);
                    _logger.LogError(message);
                    throw new Exception(message);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, (string)string.Format("MermaidsFortune generated an error, Error getting message, stack trace is:{0}", response == null ? null : (string)response.error.stackTrace));
                var er = new ErrorObject() { error = new Error() { message = ex.Message, stackTrace = ex.StackTrace } };
                result = Json.Encode(er);
            }
            return result;
        }

        private void ValidateRTP(Config platformRequest)
        {
            if (platformRequest.rtp == 0)
            {
                throw new Exception("Rtp shouldn't be empty");
            }
            else if (platformRequest.rtp != 96.0 && platformRequest.rtp != 94.0)
            {
                throw new Exception("Rtp " + platformRequest.rtp + " is not valid to this game");
            }
        }
    }
}
