@echo off
if exist .env (
    echo ���ݼҲզw�˧���...
    call npm install
    call npm install -g nodemon
    echo �}�Ҧ��A��...
    nodemon --signal SIGUSR2 --ignore public/ 
) else (
    echo �|���]�w�����ܼơA�p�G���|�]�w.env�ɡA�i�H�ϥΧڭ̪������ܼƤu��A�����G
    echo ''
)
pause