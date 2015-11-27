using System.Web;
using Secken.ServerAuth.Framework;
using Secken.ServerAuth.Models;
using Secken.ServerAuth.Models.Request;
using Secken.ServerAuth.Models.Response;

namespace Secken.ServerAuth.Demo
{
    /// <summary>
    /// YcAuth 的摘要说明
    /// </summary>
    public class YcAuth : HttpTaskAsyncHandler
    {
        // 需要去洋葱开发者中心新建一个类型为公有云的应用，创建完成之后，将对应的AppId+AppKey填过来
        private RequestForServerAuthKey _thisRequestServerAuthKey = new RequestForServerAuthKey
        {
            AppId = "",
            AppKey = ""
        };
        /// <summary>
        /// ThisRequestServerSdkKey
        /// </summary>
        public RequestForServerAuthKey ThisRequestServerAuthKey
        {
            #region ThisRequestServerSdkKey
            get
            {
                return _thisRequestServerAuthKey;
            }
            set
            {
                if (Equals(_thisRequestServerAuthKey, value)) return;
                _thisRequestServerAuthKey = value;
            }
            #endregion
        }


        /// <summary>
        /// 这个根据自己业务来，Demo中用它来做登录的Cookie Token
        /// </summary>
        private static string _nowToken = "";

        private static string _nowLoginCookieKey = "Login";

        public async override System.Threading.Tasks.Task ProcessRequestAsync(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string resposeStr = "";
            var action = context.Request[ParaForServerAuth.ActionKeyName];
            switch (action)
            {
                case ParaForServerAuth.ActionForAskYangAuthPush:
                    {
                        #region ActionForAskYangAuthPush

                        var thisRequestUserId = context.Request[ParaForServerAuth.UserIdKeyName];
                        if (!string.IsNullOrEmpty(thisRequestUserId))
                        {
                            // 准备请求参数类
                            var thisRequestYangAuthPush = new RequestForServerAuthPush(ThisRequestServerAuthKey)
                            {
                                AuthType = MethodForServerAuth.FaceVerify,
                                UserId = thisRequestUserId,
                                ActionType = TypeForAuthAction.Login,
                                UserName = "举个例子",
                                UserIpAddress = "23.545.656.6",
                            };
                            // 发起推送验证的方法
                            var thisResponseYangAuthPush = await ServerAuthProvider.Current.Action<ResponseForServerAuthPush>(ServerAuthProviderType.AskYangAuthPush, thisRequestYangAuthPush);
                            // 发起推送验证的结果
                            if (thisResponseYangAuthPush != null)
                            {
                                if (thisResponseYangAuthPush.IsLegal)
                                {
                                    // 根据需要返回
                                    resposeStr = thisResponseYangAuthPush.Serialize();
                                }
                            }
                        }

                        #endregion
                    }
                    break;
                case ParaForServerAuth.ActionForGetYcAuthQrCode:
                    {
                        #region ActionForGetYcAuthQrCode

                        // 准备请求参数类
                        var thisRequestYangAuthQrCode = new RequestForServerAuthQrCode(ThisRequestServerAuthKey)
                        {
                            AuthType = MethodForServerAuth.FaceVerify,
                        };
                        // 调用请求二维码的方法
                        var thisResponseYangAuthQrCode = await ServerAuthProvider.Current.Action<ResponseForServerAuthQrCode>(ServerAuthProviderType.GetYangAuthQrCode, thisRequestYangAuthQrCode);
                        // 获取二维码方法的结果
                        if (thisResponseYangAuthQrCode != null)
                        {
                            if (thisResponseYangAuthQrCode.IsLegal)
                            {
                                // 根据需要返回
                                resposeStr = thisResponseYangAuthQrCode.Serialize();
                            }
                        }

                        #endregion
                    }
                    break;
                case ParaForServerAuth.ActionForCheckYcAuthResult:
                    {
                        #region ActionForCheckYcAuthResult

                        var thisRequestEventId = context.Request[ParaForServerAuth.EventIdKeyName];
                        if (!string.IsNullOrEmpty(thisRequestEventId))
                        {
                            // 准备请求参数类
                            var thisRequestYangAuthResult = new RequestForServerAuthResult(ThisRequestServerAuthKey)
                            {
                                EventId = thisRequestEventId
                            };
                            // 调用查询事件结果的方法
                            var thisResponseYangAuthResult = await ServerAuthProvider.Current.Action<ResponseForServerAuthResult>(ServerAuthProviderType.CheckYangAuthResult, thisRequestYangAuthResult);
                            // 调用查询事件结果的结果
                            if (thisResponseYangAuthResult != null)
                            {
                                if (thisResponseYangAuthResult.IsLegal)
                                {
                                    // 如果这个UserId和库里面绑定的UserId一致，那就表示可以让他通过
                                    // 如果这个UserId在库里面查询不到，就可以理解为这是绑定流程。
                                    if (Equals(thisResponseYangAuthResult.UserId, ""))
                                    {
                                        // 根据需要返回
                                        _nowToken = thisResponseYangAuthResult.UserId;
                                        context.Response.SetCookie(new HttpCookie(_nowLoginCookieKey, _nowToken));
                                        resposeStr = thisResponseYangAuthResult.Serialize();
                                    }
                                    else
                                    {
                                        thisResponseYangAuthResult.Code = ParaForServerAuth.CodeForIllegalForPermission;
                                        thisResponseYangAuthResult.UserId = null;
                                        resposeStr = thisResponseYangAuthResult.Serialize();
                                    }
                                }
                                else
                                {
                                    thisResponseYangAuthResult.Code = ParaForServerAuth.CodeForIllegalForReturn;
                                    resposeStr = thisResponseYangAuthResult.Serialize();
                                }
                            }
                        }

                        #endregion
                    }
                    break;
            }

            context.Response.Write(resposeStr);
        }
    }
}