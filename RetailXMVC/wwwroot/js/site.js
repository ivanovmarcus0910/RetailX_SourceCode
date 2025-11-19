
document.addEventListener("DOMContentLoaded", function () {
    var tenantIdInput = document.getElementById("currentTenantId");

    if (tenantIdInput) {
        var tenantId = tenantIdInput.value;
        console.log("Initializing SignalR for Tenant: " + tenantId);

        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub").build();

        connection.on("ReceiveJoinRequest", function (userName, email) {
            showNotification(userName, email);

            if (window.location.href.includes("PendingRequests")) {
                setTimeout(() => location.reload(), 5000);
            }
        });

        connection.start().then(function () {
            console.log("SignalR Connected Successfully!");

            connection.invoke("JoinTenantGroup", tenantId).catch(function (err) {
                return console.error("Error joining group: " + err.toString());
            });

        }).catch(function (err) {
            return console.error("SignalR Connection Error: " + err.toString());
        });
    }
});

function showNotification(userName, email) {
    var message = `<strong>${userName}</strong> (${email}) vừa gửi yêu cầu gia nhập công ty.`;

    var toastBody = document.getElementById("toastMessage");
    if (toastBody) toastBody.innerHTML = message;

    var toastEl = document.getElementById('liveToast');
    if (toastEl) {
        var toast = new bootstrap.Toast(toastEl);
        toast.show();
    }
}
