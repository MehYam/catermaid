<?php
require 'utils.php';
session_start();
if (!authenticated()) {
    header('Location: login.php');
} else {
    ?>

    <!DOCTYPE html>
    <html>
        <head>
            <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
            <title>Catermaid</title>
    	<style>
    	    img.kira {
    		padding: 2px;
    		border: 3px solid #fff;
    		background: #fff;
    	    }
    	    img.kira:hover {
    		border-color: #dbd5c5;
    		background: #dbd5c5;
    	    }
    	</style>
        </head>
        <body>
    	<div id="header">
    	    <div id="logo">
    		<img src="images/catermaid.ico" alt="CaterMaid" />
		<font style="color:green;font-size:200%;font-family:Tahoma;">Catermaid</font>
    	    </div>
	    <hr>
	    <br/>
    	</div>

    	Hello, <?php echo $_SESSION['user_email']; ?>.  You can now access Catermaid.  :)

        </body>
    </html>

    <?php
}
?>