echo wait for the package to install
call npm install
call npm install -g nodemon
echo starting the server...
nodemon --ignore public/