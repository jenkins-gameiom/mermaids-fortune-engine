using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common;

namespace AGS.Slots.MermaidsFortune.WebAPI
{
    public class RequestExecutionContext : IRequestContext
    {
        private const string KEY_ID = "ExecutionContext_ID";
        private const string KEY_State = "ExecutionContext_State";
        private const string KEY_Config = "ExecutionContext_Config";
        private const string KEY_MathFile = "ExecutionContext_MathFile";
        private const string KEY_RequestItems = "ExecutionContext_RequestItems";

        private readonly IHttpContextAccessor _contextAccessor;

        public RequestExecutionContext(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;

            if (Id == null)
            {
                Id = Guid.NewGuid().ToString();
            }
        }

        public string Id
        {
            get
            {
                if (_contextAccessor.HttpContext.Items.ContainsKey(KEY_ID))
                {
                    return (string)_contextAccessor.HttpContext.Items[KEY_ID];
                }

                return null;
            }
            private set
            {
                if (!_contextAccessor.HttpContext.Items.ContainsKey(KEY_ID))
                {
                    _contextAccessor.HttpContext.Items[KEY_ID] = value;
                }
                else
                {
                    throw new ArgumentException("Cannot set Request ID after it has been set");
                }
            }
        }

        public IStateItems State 
        {
            get
            {
                if (_contextAccessor.HttpContext.Items.ContainsKey(KEY_State))
                {
                    return (IStateItems)_contextAccessor.HttpContext.Items[KEY_State];
                }

                return null;
            }
            set
            {
                _contextAccessor.HttpContext.Items[KEY_State] = value;
            }
        }
        public Config Config
        {
            get
            {
                if (_contextAccessor.HttpContext.Items.ContainsKey(KEY_Config))
                {
                    return (Config)_contextAccessor.HttpContext.Items[KEY_Config];
                }
                return null;
            }
            set
            {
                _contextAccessor.HttpContext.Items[KEY_Config] = value;
            }
        }

        public IMathFile MathFile
        {
            get
            {
                if (_contextAccessor.HttpContext.Items.ContainsKey(KEY_MathFile))
                {
                    return (IMathFile)_contextAccessor.HttpContext.Items[KEY_MathFile];
                }
                return null;
            }
            set
            {
                _contextAccessor.HttpContext.Items[KEY_MathFile] = value;
            }
        }

        public RequestItems RequestItems
        {
            get
            {
                if (_contextAccessor.HttpContext.Items.ContainsKey(KEY_RequestItems))
                {
                    return (RequestItems)_contextAccessor.HttpContext.Items[KEY_RequestItems];
                }
                return null;
            }
            set
            {
                _contextAccessor.HttpContext.Items[KEY_RequestItems] = value;
            }
        }

        public int GetDenom()
        {
            if (!(RequestItems.isFreeSpin || RequestItems.action == ActionType.pick.ToString() || RequestItems.action == ActionType.jackpotpick.ToString()) || State.lastState == null || State.lastState.denom == null || State.lastState.denom.Value == 0)
            {
                return RequestItems.denom;
            }
            else
            {
                return State.lastState.denom.Value;
            }
        }

        public int GetBetAmount()
        {
            if (!(RequestItems.isFreeSpin || RequestItems.action == ActionType.pick.ToString() || RequestItems.action == ActionType.jackpotpick.ToString()) || State.lastState == null || State.lastState.betAmount == null || State.lastState.betAmount.Value == 0)
            {
                return RequestItems.betAmount;
            }
            else
            {
                return State.lastState.betAmount.Value;
            }
        }
    }
}
