
const sql = require('./asyncDB');

var request = async function(stmt){
    var result={};
    await sql( stmt )
        .then((data) => {
            result = data.rows;
        }, (error) => {
            result = null;
        });
		
    return result;
}

module.exports = {request};