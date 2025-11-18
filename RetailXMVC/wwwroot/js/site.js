
//document.addEventListener("DOMContentLoaded", function () {

//    // 1. Kiểm tra xem có phải Owner đang đăng nhập không (Check hidden input)
//    var tenantIdInput = document.getElementById("currentTenantId");

//    if (tenantIdInput) {
//        var tenantId = tenantIdInput.value;
//        console.log("Initializing SignalR for Tenant: " + tenantId);

//        // 2. Khởi tạo kết nối SignalR
//        var connection = new signalr.HubConnectionBuilder()
//            .withUrl("/notificationHub") // Đảm bảo đường dẫn khớp với Program.cs
//            .withAutomaticReconnect() // Tự động kết nối lại nếu rớt mạng
//            .build();

//        // 3. Định nghĩa hành động khi nhận thông báo "ReceiveJoinRequest"
//        connection.on("ReceiveJoinRequest", function (userName, email) {
//            showNotification(userName, email);

//            // Cập nhật badge thông báo (nếu có)
//            var badge = document.getElementById("requestCountBadge");
//            if (badge) badge.style.display = "inline-block";

//            // Nếu đang ở trang duyệt, reload để thấy data mới
//            if (window.location.href.includes("PendingRequests")) {
//                setTimeout(() => location.reload(), 2000); // Đợi 2s cho user đọc thông báo rồi reload
//            }
//        });

//        // 4. Bắt đầu kết nối
//        connection.start().then(function () {
//            console.log("SignalR Connected Successfully!");

//            // Tham gia vào Group của Tenant mình
//            connection.invoke("JoinTenantGroup", tenantId).catch(function (err) {
//                return console.error("Error joining group: " + err.toString());
//            });

//        }).catch(function (err) {
//            return console.error("SignalR Connection Error: " + err.toString());
//        });
//    }
//});

//// Hàm hiển thị Toast Bootstrap
//function showNotification(userName, email) {
//    var message = `<strong>${userName}</strong> (${email}) vừa gửi yêu cầu gia nhập công ty.`;

//    // 1. Gán nội dung
//    var toastBody = document.getElementById("toastMessage");
//    if (toastBody) toastBody.innerHTML = message;

//    // 2. Show Toast
//    var toastEl = document.getElementById('liveToast');
//    if (toastEl) {
//        var toast = new bootstrap.Toast(toastEl);
//        toast.show();
//    }
//}
