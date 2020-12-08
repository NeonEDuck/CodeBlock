const { exec } = require("child_process");
var express = require('express');
var router = express.Router();
var pulling = false;

router.get('/', function(req, res, next) {
	var pass = req.query.pass;
	if ( pass === process.env.GIT_PASSWORD ) {
		if ( pulling === false ) {
			pulling = true
			
			exec("git fetch", (error, stdout, stderr) => {
				if (error) {
					console.log(`error: ${error.message}`);
					return;
				}
				console.log('fetching...');

				exec("git reset --hard origin/master", (error, stdout, stderr) => {
					if (error) {
						console.log(`error: ${error.message}`);
						return;
					}
					if (stderr) {
						console.log(`stderr: ${stderr}`);
						return;
					}
					console.log(`stdout: ${stdout}`);
				})
			})
			
			pulling = false
		}
	}
	res.redirect('back');
});

router.post('/', function(req, res, next) {
	var pass = req.query.pass;
	if ( pass === process.env.GIT_PASSWORD ) {
		if ( pulling === false ) {
			pulling = true
			
			exec("git fetch", (error, stdout, stderr) => {
				if (error) {
					console.log(`error: ${error.message}`);
					return;
				}
				console.log('fetching...');

				exec("git reset --hard origin/master", (error, stdout, stderr) => {
					if (error) {
						console.log(`error: ${error.message}`);
						return;
					}
					if (stderr) {
						console.log(`stderr: ${stderr}`);
						return;
					}
					console.log(`stdout: ${stdout}`);
				})
			})
			
			pulling = false
		}
	}
	res.redirect('back');
});

module.exports = router;