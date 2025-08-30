<body style="
    background: url('${url.resourcesPath}/img/custom-bg.jpg') no-repeat center center fixed;
    background-size: cover;
    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    margin: 0;
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: center;
">

<!-- Semi-transparent overlay for readability -->
<div style="
    position: absolute;
    top: 0; left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0,0,0,0.35);
    z-index: 0;
"></div>

<div class="login-container" style="
    position: relative;
    z-index: 1;
    max-width: 400px;
    width: 90%;
    padding: 40px 30px;
    background: rgba(255,255,255,0.95);
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.25);
">

    <h2 style="text-align:center; margin-bottom: 30px; font-weight: 600; color: #1f2937;">
        ${msg("loginTitle", realm.displayName)}
    </h2>

    <form id="kc-form-login" action="${url.loginAction}" method="post">
        <div class="field" style="margin-bottom: 20px;">
            <label for="username" style="display:block; margin-bottom:6px; font-weight:500; color:#374151;">
                ${msg("username")}
            </label>
            <input id="username" name="username" type="text" value="${(username)!''}" required autofocus
                style="width:100%; padding:12px; border:1px solid #d1d5db; border-radius:8px; font-size:14px; box-sizing:border-box;"
            />
        </div>

        <div class="field" style="margin-bottom:25px;">
            <label for="password" style="display:block; margin-bottom:6px; font-weight:500; color:#374151;">
                ${msg("password")}
            </label>
            <input id="password" name="password" type="password" required
                style="width:100%; padding:12px; border:1px solid #d1d5db; border-radius:8px; font-size:14px; box-sizing:border-box;"
            />
        </div>

        <button id="kc-login" name="login" type="submit"
            style="width:100%; padding:14px; border:none; border-radius:8px; background-color:#2563eb; color:white; font-weight:600; font-size:16px; cursor:pointer; transition: background 0.3s;"
            onmouseover="this.style.backgroundColor='#1e40af';"
            onmouseout="this.style.backgroundColor='#2563eb';"
        >
            ${msg("doLogIn")}
        </button>
    </form>
</div>
</body>
