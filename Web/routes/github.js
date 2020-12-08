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
					pulling = false
					return;
				}
				console.log('fetching...');

				exec("git reset --hard origin/master", (error, stdout, stderr) => {
					pulling = false
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
		}
	}
	res.redirect('back');
});

router.post('/', function(req, res, next) {
	var pass = req.query.pass;
	var ref = req.body.ref;


	if (ref === 'refs/heads/master') {
		if ( pass === process.env.GIT_PASSWORD ) {
			if ( pulling === false ) {
				pulling = true
				
				exec("git fetch", (error, stdout, stderr) => {
					if (error) {
						console.log(`error: ${error.message}`);
						pulling = false
						return;
					}

					exec("git reset --hard origin/master", (error, stdout, stderr) => {
						pulling = false
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
				res.send('received.');
			}
			else {
				res.send('cancel, because the git is already pulling.');
			}
		}
		else {
			res.send('bad key.');
		}
	}
	else {
		console.log('ignore non-master update.');
		res.send('ignore non-master update.');
	}
});

module.exports = router;