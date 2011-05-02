<?php

function loadDB() {
    $conn = mysql_connect('localhost', 'ahost919', 'magic8magic');
    if (!$conn) {
	die('Could not connect to mySQL: ' . mysql_error());
    } elseif (!mysql_select_db("ahost919_catermaid", $conn)) {
	die('Unable to select db: ' . mysql_error());
    } else {
	return $conn;
    }
}

function wasOpenIDValidatedBefore($oid) {
    //make sure the "openid" cookie value matches what we have in our DB
    //otherwise anyone with a cookie editor can create the "openid" key with any random value
    $conn = loadDB();
    if ($conn) {
	$query = "SELECT email from users WHERE openid='$oid'";
	$result = mysql_query($query);
	while ($row = mysql_fetch_array($result)) {
	    $_SESSION['user_email'] = $row['email'];
	    return true;
	}
	mysql_close($conn);
    }

    return false;
}

//check if we've saved a valid cookie
function authenticated() {
    return isset($_COOKIE['openid']) && wasOpenIDValidatedBefore($_COOKIE['openid']);
}

function validateID($oid, $email) {

    //whitelist
    $allowedUsers = array('kerbumble@gmail.com', 'cdn.kai@gmail.com', 'briantu81@gmail.com');

    if (in_array($email, $allowedUsers)) {
	$conn = loadDB();
	if ($conn)
	{
	    $query = "SELECT email from users WHERE openid='$oid'";
	    $result = mysql_query($query);
	    $row = mysql_fetch_array($result);
	    if(!$row)
	    {
		$query = "INSERT INTO users (openid, email) VALUES ('$oid', '$email')";
		if (!mysql_query($query)) {
		    die('unable to add, error: ' . mysql_error());
		}
	    }
	    $_SESSION['user_email'] = $email;
	    mysql_close($conn);
	}

	setcookie('openid', $oid, time() + 60 * 60 * 24 * 30); //let the cookie last 30 days

	return true;
    }

    return false;
}

?>
