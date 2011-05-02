<?php
require 'LightOpenID/openid.php';
require 'utils.php';

try {
    $openid = new LightOpenID;
    if (!$openid->mode) {
	if (isset($_GET['login'])) {
	    $openid->required = array('contact/email');
	    $openid->identity = 'https://www.google.com/accounts/o8/id';
	    header('Location: ' . $openid->authUrl());
	}
	?>
	<html>
	    <body>
		<div id="header">
		    <div id="logo">
			<img src="images/catermaid.ico" alt="CaterMaid" />
			<font style="color:green;font-size:200%;font-family:Tahoma;">Catermaid</font>
		    </div>
		    <hr>
		    <br/>
		</div>

		<form action="?login" method="post">
							Log in with Google: <button style="background-color:white;border-style:none"><img class="kira" src="images/google-hand-drawn.png"/></button>
		</form>
		<?php
	    } elseif ($openid->mode == 'cancel') {
		echo 'User has canceled authentication!';
	    } elseif ($openid->validate()) {
		$attrs = $openid->getAttributes();
		$userEmail = $attrs['contact/email'];
		if (validateID($openid->identity, $userEmail)) {
		    header('Location: home.php');
		} else {
		    echo "Sorry, $userEmail, you're not allowed access to Catermaid.  <br/> If you'd like to <em>sign out</em> of your current Google account and sign into another account, <a href='http://www.google.com/accounts/Logout?continue=https://www.google.com/accounts/ServiceLogin?hl=en'>click here</a>.";
		    //mail('kerbumble@yahoo.com', 'catermaid user denied', 'Following email wanted to access catermaid but was denied: ' . $userEmail . '\n');
		}
	    } else {
		echo 'User ' . $openid->identity . 'has not logged in.';
	    }
	} catch (ErrorException $e) {
	    echo $e->getMessage();
	}
	?>
    </body>
</html>