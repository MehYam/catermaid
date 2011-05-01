<?php

//whitelist
$allowedUsers = array('kerbumble@gmail.com', 'cdn.kai@gmail.com', 'briantu81@gmail.com');

function isValidID() {
    //TODO: make sure the OpenID identifier is one we're validated before (put in DB users table)
    return true;
}

//check if we've saved a valid cookie
function authenticated() {
    return isset($_COOKIE['openid']) && isValidID($_COOKIE['openid']);
}

function validateID(&$oid, &$email) {

    if (in_array($userEmail, $allowedUsers)) {
	setcookie('openid', $openid->identity, time() + 60 * 60 * 24 * 30); //let the cookie last 30 days
	$con = mysql_connect('localhost');
	if (!$con)
	{
	    die('Could not connect: ' . mysql_error());
	}
	else
	{
	    die('success');
	
	}
	return true;
    }

    return false;
}

?>
