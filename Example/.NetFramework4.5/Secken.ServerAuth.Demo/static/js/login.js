

$(function() {
    var isPass = false;

    function verify(eid) {
        var obj = {
            eid: eid,
            Action: "CheckYcAuthResult"
        }
        obj.t = (new Date()).getTime();
        $.getJSON("/YcAuth.ashx", obj, function(result) {
            if (!isPass) {
                if (result.code != "0") {
                    if (result.code == "-4") {
                        isPass = true;
                    } else if (result.code == "-7") {
                        show = false;
                        if (result.status == "603") {
                            stateChange("二维码过期！");
                            bindRefresh();
                            isPass = true;
                        } else if (result.status == "602") {
                            stateChange("等待用户响应！");
                            isPass = false;
                        } else if (result.status == "606") {
                            stateChange("启用了回调地址！");
                            isPass = true;
                        } else if (result.status == "601") {
                            stateChange("用户拒绝授权！");
                            window.location.reload();
                            isPass = false;
                        }
                    }
                    if (!isPass) {
                        setTimeout(verify(eid), 2000);
                    }
                } else {
                    isPass = true;
                    window.location.href = "/push.html";
                }
            }
        });
    };

    stateChange("Is Loading QrCode...");
    var obj = {
        Action: "GetYcAuthQrCode"
    }
    obj.t = (new Date()).getTime();
    $.getJSON("/YcAuth.ashx", obj, function(result) {
        if (result.code == "0") {
            eid = result.event_id;
            $("#qr_img").attr("src", result.qrcode_url);
            hideState();
            verify(eid);
        } else if (result.code == "-1") {
            stateChange("获取失败!");
        }
    });

    Window.verifyOneClick = function() {
        var obj = {
            Action: "GetYcAuthOneClick"
        }
        obj.t = (new Date()).getTime();
        $.getJSON("/YcAuth.ashx", obj, function(result) {
            if (result.code == "0") {
                eid = result.event_id;
                hideState();
                verify(eid);
            } else if (result.code == "-1") {
                stateChange("获取失败!");
            }
        });
    }

    function bindRefresh() {
        $(".hightLightSpan").bind("click", function() {
            window.location.reload();
        });
    }
});

function stateChange(tip) {
    $(".qrCodeOverdue").html(tip);
    $(".qrCodeOverdue").show();
}

function hideState() {
    $(".qrCodeOverdue").hide();
}