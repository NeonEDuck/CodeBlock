var gameInstance = UnityLoader.instantiate("unityContainer", "unity/Build/Builds.json", {onProgress: UnityProgress});
var jsonString = $('#json').text();
$('#json').remove();

function UnityStartup() {
	
	console.log('sending...');
	gameInstance.SendMessage('NetworkManager','SetVariables', jsonString);
}

function SendJson(str) {
	
	console.log('sending...');
	gameInstance.SendMessage('NetworkManager','GetJson', str);
}