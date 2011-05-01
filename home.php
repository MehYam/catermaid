<?php
    require 'utils.php';
    if( !authenticated() )
    {
	header('Location: login.php');
    }
    else {
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
        <img src="images/catermaid.ico"/>
        <font style="color:green;font-size:200%;font-family:Tahoma;">Catermaid</font><br/>
        <hr>
        <br/>

	Authenticated.

    </body>
</html>

<?php
    }
?>