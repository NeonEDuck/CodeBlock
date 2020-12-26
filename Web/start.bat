@echo off
if exist .env (
    echo 等待模組安裝完成...
    call npm install
    call npm install -g nodemon
    echo 開啟伺服器...
    nodemon --signal SIGUSR2 --ignore public/ 
) else (
    echo 尚未設定環境變數，如果不會設定.env檔，可以使用我們的環境變數工具，網站：
    echo ''
)
pause