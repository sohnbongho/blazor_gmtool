using Messages;
using NatsMessages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Helper
{
    public partial class PacketLogHelper    
    {
        private static readonly Lazy<PacketLogHelper> lazy = new Lazy<PacketLogHelper>(() => new PacketLogHelper());
        public static PacketLogHelper Instance { get { return lazy.Value; } }
        private Dictionary<MessageWrapper.PayloadOneofCase, Func<MessageWrapper, string>> _userHandlers;
        private Dictionary<NatsMessageWrapper.PayloadOneofCase, Func<NatsMessageWrapper, string>> _natsHandlers;

        public PacketLogHelper() 
        {
            _userHandlers = new Dictionary<MessageWrapper.PayloadOneofCase, Func<MessageWrapper, string>>
            {
                {MessageWrapper.PayloadOneofCase.ConnectedResponse, (data) => JsonConvert.SerializeObject(data.ConnectedResponse) },
                {MessageWrapper.PayloadOneofCase.KeepAliveNoti, (data) => JsonConvert.SerializeObject(data.KeepAliveNoti) },
                
                {MessageWrapper.PayloadOneofCase.SellItemRequest, (data) => JsonConvert.SerializeObject(data.SellItemRequest) },
                {MessageWrapper.PayloadOneofCase.SellItemResponse, (data) => JsonConvert.SerializeObject(data.SellItemResponse) },
                                
                {MessageWrapper.PayloadOneofCase.ZoneChatRequest, (data) => JsonConvert.SerializeObject(data.ZoneChatRequest) },
                {MessageWrapper.PayloadOneofCase.ZoneChatNoti, (data) => JsonConvert.SerializeObject(data.ZoneChatNoti) },

                {MessageWrapper.PayloadOneofCase.ZoneEnterRequest, (data) => JsonConvert.SerializeObject(data.ZoneEnterRequest) },
                {MessageWrapper.PayloadOneofCase.ZoneEnterResponse, (data) => JsonConvert.SerializeObject(data.ZoneEnterResponse) },
                {MessageWrapper.PayloadOneofCase.ZoneEnterNoti, (data) => JsonConvert.SerializeObject(data.ZoneEnterNoti) },
                {MessageWrapper.PayloadOneofCase.ZoneCharacterList, (data) => JsonConvert.SerializeObject(data.ZoneCharacterList) },

                {MessageWrapper.PayloadOneofCase.ZoneLeaveNoti, (data) => JsonConvert.SerializeObject(data.ZoneLeaveNoti) },

                {MessageWrapper.PayloadOneofCase.MoveRequest, (data) => JsonConvert.SerializeObject(data.MoveRequest) },
                {MessageWrapper.PayloadOneofCase.MoveNoti, (data) => JsonConvert.SerializeObject(data.MoveNoti) },

                {MessageWrapper.PayloadOneofCase.BehaviorRequest, (data) => JsonConvert.SerializeObject(data.BehaviorRequest) },
                {MessageWrapper.PayloadOneofCase.BehaviorNoti, (data) => JsonConvert.SerializeObject(data.BehaviorNoti) },

                {MessageWrapper.PayloadOneofCase.ObjectListNoti, (data) => JsonConvert.SerializeObject(data.ObjectListNoti) },                
                {MessageWrapper.PayloadOneofCase.ObjectUseRequest, (data) => JsonConvert.SerializeObject(data.ObjectUseRequest) },
                {MessageWrapper.PayloadOneofCase.ObjectUseResponse, (data) => JsonConvert.SerializeObject(data.ObjectUseResponse) },
                {MessageWrapper.PayloadOneofCase.ObjectUseNoti, (data) => JsonConvert.SerializeObject(data.ObjectUseNoti) },

                {MessageWrapper.PayloadOneofCase.SnowObjectListNoti, (data) => JsonConvert.SerializeObject(data.SnowObjectListNoti) },
                {MessageWrapper.PayloadOneofCase.SnowObjectUseRequest, (data) => JsonConvert.SerializeObject(data.SnowObjectUseRequest) },                
                {MessageWrapper.PayloadOneofCase.SnowObjectUseResponse, (data) => JsonConvert.SerializeObject(data.SnowObjectUseResponse) },                

                {MessageWrapper.PayloadOneofCase.ObjectUseEndRequest, (data) => JsonConvert.SerializeObject(data.ObjectUseEndRequest) },
                {MessageWrapper.PayloadOneofCase.ObjectUseEndResponse, (data) => JsonConvert.SerializeObject(data.ObjectUseEndResponse) },
                {MessageWrapper.PayloadOneofCase.ObjectUseEndNoti, (data) => JsonConvert.SerializeObject(data.ObjectUseEndNoti) },

                {MessageWrapper.PayloadOneofCase.ObjectStateRequest, (data) => JsonConvert.SerializeObject(data.ObjectStateRequest) },
                {MessageWrapper.PayloadOneofCase.ObjectStateNoti, (data) => JsonConvert.SerializeObject(data.ObjectStateNoti) },

                {MessageWrapper.PayloadOneofCase.ObjectOwnTypeRequest, (data) => JsonConvert.SerializeObject(data.ObjectOwnTypeRequest) },
                {MessageWrapper.PayloadOneofCase.ObjectOwnTypeNoti, (data) => JsonConvert.SerializeObject(data.ObjectOwnTypeNoti) },

                {MessageWrapper.PayloadOneofCase.ObjectTransformRequest, (data) => JsonConvert.SerializeObject(data.ObjectTransformRequest) },
                {MessageWrapper.PayloadOneofCase.ObjectTransformNoti, (data) => JsonConvert.SerializeObject(data.ObjectTransformNoti) },

                {MessageWrapper.PayloadOneofCase.PlayerActionRequest, (data) => JsonConvert.SerializeObject(data.PlayerActionRequest) },
                {MessageWrapper.PayloadOneofCase.PlayerActionNoti, (data) => JsonConvert.SerializeObject(data.PlayerActionNoti) },

                {MessageWrapper.PayloadOneofCase.PlayerBallActionRequest, (data) => JsonConvert.SerializeObject(data.PlayerBallActionRequest) },
                {MessageWrapper.PayloadOneofCase.PlayerBallActionNoti, (data) => JsonConvert.SerializeObject(data.PlayerBallActionNoti) },

                {MessageWrapper.PayloadOneofCase.SysMessageRequest, (data) => JsonConvert.SerializeObject(data.SysMessageRequest) },
                {MessageWrapper.PayloadOneofCase.SysMessageNoti, (data) => JsonConvert.SerializeObject(data.SysMessageNoti) },

                // Room관련
                {MessageWrapper.PayloadOneofCase.RoomCreateRequest, (data) => JsonConvert.SerializeObject(data.RoomCreateRequest) },
                {MessageWrapper.PayloadOneofCase.RoomCreateResponse, (data) => JsonConvert.SerializeObject(data.RoomCreateResponse) },
                {MessageWrapper.PayloadOneofCase.RoomListRequest, (data) => JsonConvert.SerializeObject(data.RoomListRequest) },
                {MessageWrapper.PayloadOneofCase.RoomListResponse, (data) => JsonConvert.SerializeObject(data.RoomListResponse) },                
                {MessageWrapper.PayloadOneofCase.RoomEnterRequest, (data) => JsonConvert.SerializeObject(data.RoomEnterRequest) },
                {MessageWrapper.PayloadOneofCase.RoomEnterResponse, (data) => JsonConvert.SerializeObject(data.RoomEnterResponse) },
                {MessageWrapper.PayloadOneofCase.RoomLeaveRequest, (data) => JsonConvert.SerializeObject(data.RoomLeaveRequest) },
                {MessageWrapper.PayloadOneofCase.RoomLeaveResponse, (data) => JsonConvert.SerializeObject(data.RoomLeaveResponse) },
                {MessageWrapper.PayloadOneofCase.RoomCharListRequest, (data) => JsonConvert.SerializeObject(data.RoomCharListRequest) },
                {MessageWrapper.PayloadOneofCase.RoomCharListResponse, (data) => JsonConvert.SerializeObject(data.RoomCharListResponse) },
                // 아이템 인벤 확장
                {MessageWrapper.PayloadOneofCase.ItemInvenExpandRequest, (data) => JsonConvert.SerializeObject(data.ItemInvenExpandRequest) },
                {MessageWrapper.PayloadOneofCase.ItemInvenExpandResponse, (data) => JsonConvert.SerializeObject(data.ItemInvenExpandResponse) },
                
                // 룸 방장 변경
                {MessageWrapper.PayloadOneofCase.RoomMasterChangeRequest, (data) => JsonConvert.SerializeObject(data.RoomMasterChangeRequest) },
                {MessageWrapper.PayloadOneofCase.RoomMasterChangeResponse, (data) => JsonConvert.SerializeObject(data.RoomMasterChangeResponse) },
                {MessageWrapper.PayloadOneofCase.RoomMasterChangeNoti, (data) => JsonConvert.SerializeObject(data.RoomMasterChangeNoti) },
                // 룸 팀 변경
                {MessageWrapper.PayloadOneofCase.RoomTeamChangeRequest, (data) => JsonConvert.SerializeObject(data.RoomTeamChangeRequest) },
                {MessageWrapper.PayloadOneofCase.RoomTeamChangeResponse, (data) => JsonConvert.SerializeObject(data.RoomTeamChangeResponse) },
                {MessageWrapper.PayloadOneofCase.RoomTeamChangeNoti, (data) => JsonConvert.SerializeObject(data.RoomTeamChangeNoti) },
                // 룸 게임 시작
                {MessageWrapper.PayloadOneofCase.RoomGameStartRequest, (data) => JsonConvert.SerializeObject(data.RoomGameStartRequest) },
                {MessageWrapper.PayloadOneofCase.RoomGameStartResponse, (data) => JsonConvert.SerializeObject(data.RoomGameStartResponse) },
                {MessageWrapper.PayloadOneofCase.RoomGameStartNoti, (data) => JsonConvert.SerializeObject(data.RoomGameStartNoti) },
                // 룸 게임 종료
                {MessageWrapper.PayloadOneofCase.RoomGameEndRequest, (data) => JsonConvert.SerializeObject(data.RoomGameEndRequest) },
                {MessageWrapper.PayloadOneofCase.RoomGameEndResponse, (data) => JsonConvert.SerializeObject(data.RoomGameEndResponse) },
                {MessageWrapper.PayloadOneofCase.RoomGameEndNoti, (data) => JsonConvert.SerializeObject(data.RoomGameEndNoti) },
                // 룸 옵션 변경
                {MessageWrapper.PayloadOneofCase.RoomOptionChangedRequest, (data) => JsonConvert.SerializeObject(data.RoomOptionChangedRequest) },
                {MessageWrapper.PayloadOneofCase.RoomOptionChangedResponse, (data) => JsonConvert.SerializeObject(data.RoomOptionChangedResponse) },
                {MessageWrapper.PayloadOneofCase.RoomOptionChangedNoti, (data) => JsonConvert.SerializeObject(data.RoomOptionChangedNoti) },
                // 룸 채팅
                {MessageWrapper.PayloadOneofCase.RoomChatRequest, (data) => JsonConvert.SerializeObject(data.RoomChatRequest) },
                {MessageWrapper.PayloadOneofCase.RoomChatNoti, (data) => JsonConvert.SerializeObject(data.RoomChatNoti) },
                // 룸 게임 하이라이트
                {MessageWrapper.PayloadOneofCase.RoomGameHighlightRequest, (data) => JsonConvert.SerializeObject(data.RoomGameHighlightRequest) },
                {MessageWrapper.PayloadOneofCase.RoomGameHighlightResponse, (data) => JsonConvert.SerializeObject(data.RoomGameHighlightResponse) },
                {MessageWrapper.PayloadOneofCase.RoomGameHighlightNoti, (data) => JsonConvert.SerializeObject(data.RoomGameHighlightNoti) },
                
                // 시즌아이템 구매
                {MessageWrapper.PayloadOneofCase.BuySeasonItemRequest, (data) => JsonConvert.SerializeObject(data.BuySeasonItemRequest) },
                {MessageWrapper.PayloadOneofCase.BuySeasonItemResponse, (data) => JsonConvert.SerializeObject(data.BuySeasonItemResponse) },
                // 아이템 장착
                {MessageWrapper.PayloadOneofCase.EquipSeasonItemRequest, (data) => JsonConvert.SerializeObject(data.EquipSeasonItemRequest) },
                {MessageWrapper.PayloadOneofCase.EquipSeasonItemResponse, (data) => JsonConvert.SerializeObject(data.EquipSeasonItemResponse) },
                {MessageWrapper.PayloadOneofCase.EquipSeasonItemNoti, (data) => JsonConvert.SerializeObject(data.EquipSeasonItemNoti) },
                // 아이템 해제
                {MessageWrapper.PayloadOneofCase.UnEquipSeasonItemRequest, (data) => JsonConvert.SerializeObject(data.UnEquipSeasonItemRequest) },
                {MessageWrapper.PayloadOneofCase.UnEquipSeasonItemResponse, (data) => JsonConvert.SerializeObject(data.UnEquipSeasonItemResponse) },
                {MessageWrapper.PayloadOneofCase.UnEquipSeasonItemNoti, (data) => JsonConvert.SerializeObject(data.UnEquipSeasonItemNoti) },                
                //눈->골드로 전환
                {MessageWrapper.PayloadOneofCase.ExchangeSnowForGoldRequest, (data) => JsonConvert.SerializeObject(data.ExchangeSnowForGoldRequest) },
                {MessageWrapper.PayloadOneofCase.ExchangeSnowForGoldResponse, (data) => JsonConvert.SerializeObject(data.ExchangeSnowForGoldResponse) },

                {MessageWrapper.PayloadOneofCase.CommnunityServerEnterRequest, (data) => JsonConvert.SerializeObject(data.CommnunityServerEnterRequest) },
                {MessageWrapper.PayloadOneofCase.CommnunityServerEnterResponse, (data) => JsonConvert.SerializeObject(data.CommnunityServerEnterResponse) },

                {MessageWrapper.PayloadOneofCase.RoomUserInvitedRequest, (data) => JsonConvert.SerializeObject(data.RoomUserInvitedRequest) },
                {MessageWrapper.PayloadOneofCase.RoomUserInvitedResponse, (data) => JsonConvert.SerializeObject(data.RoomUserInvitedResponse) },
                {MessageWrapper.PayloadOneofCase.RoomUserInvitedNoti, (data) => JsonConvert.SerializeObject(data.RoomUserInvitedNoti) },

                {MessageWrapper.PayloadOneofCase.ZoneUserInvitedRequest, (data) => JsonConvert.SerializeObject(data.ZoneUserInvitedRequest) },
                {MessageWrapper.PayloadOneofCase.ZoneUserInvitedResponse, (data) => JsonConvert.SerializeObject(data.ZoneUserInvitedResponse) },
                {MessageWrapper.PayloadOneofCase.ZoneUserInvitedNoti, (data) => JsonConvert.SerializeObject(data.ZoneUserInvitedNoti) },
            };

            _natsHandlers = new Dictionary<NatsMessageWrapper.PayloadOneofCase, Func<NatsMessageWrapper, string>>
            {
                {NatsMessageWrapper.PayloadOneofCase.ConnectedResponse, (data) => JsonConvert.SerializeObject(data.ConnectedResponse) },
            };
    }

        public string GetLogJson(MessageWrapper message)
        {            
            if(_userHandlers.TryGetValue(message.PayloadCase, out var handler))
            {
                return handler(message);
            }
            return $"{message.PayloadCase}";
        }

        public string GetNatsMessageToJson(NatsMessageWrapper message)
        {
            if (_natsHandlers.TryGetValue(message.PayloadCase, out var handler))
            {
                return handler(message);
            }
            return $"{message.PayloadCase}";
        }
    }
}
