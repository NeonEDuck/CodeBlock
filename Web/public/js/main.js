function PasswordCheck(){
    var pass = $(':input[name="password"]').val() == $(':input[name="re_pass"]').val();
    if (pass){
        // $('.passhint').prop('hidden',false);
    }
    else {
        // $('.passhint').prop('hidden',true);
        alert("wrong");
    }
}


