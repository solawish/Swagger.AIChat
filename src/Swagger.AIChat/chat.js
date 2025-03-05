document.addEventListener("DOMContentLoaded", function () {
    const chatWidget = document.createElement('div');
    chatWidget.innerHTML = `
                <div class="chat-icon" onclick="toggleChat()">💬</div>
                <div class="chat-box" id="chatBox">
                    <div class="chat-header">聊天</div>
                    <div class="chat-messages" id="chatMessages"></div>
                    <div class="chat-input">
                        <input type="text" id="chatInput" placeholder="輸入訊息..." onkeypress="handleKeyPress(event)">
                        <button onclick="sendMessage()">送出</button>
                    </div>
                </div>
            `;
    document.body.appendChild(chatWidget);
});

const guid = crypto.randomUUID();

function toggleChat() {
    const chatBox = document.getElementById('chatBox');
    chatBox.style.display = chatBox.style.display === 'flex' ? 'none' : 'flex';
}
function handleKeyPress(event) {
    if (event.key === 'Enter') {
        sendMessage();
    }
}
function sendMessage() {
    const input = document.getElementById('chatInput');
    const message = input.value.trim();
    if (!message) return;

    addMessage(message, 'user');
    input.value = '';

    fetch('/api/chat', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'authorization': 'Bearer ' + getAccessToken()
        },
        body: JSON.stringify({ message, guid })
    })
        .then(response => response.json())
        .then(data => addMessage(data.reply, 'ai'))
        .catch(error => addMessage('無法取得回應', 'ai'));
}
function addMessage(text, role) {
    const chatMessages = document.getElementById('chatMessages');
    const messageDiv = document.createElement('div');
    messageDiv.classList.add("chat-message", role);
    messageDiv.innerHTML = `<p>${text}</p>`;
    chatMessages.appendChild(messageDiv);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function getAccessToken() {
    try {
        const authorized = localStorage.getItem('authorized');
        if (!authorized) return '';

        const parsed = JSON.parse(authorized);
        if (!parsed || !parsed.oauth2 || !parsed.oauth2.token || !parsed.oauth2.token.access_token) {
            return '';
        }

        return parsed.oauth2.token.access_token;
    } catch (error) {
        return '';
    }
}