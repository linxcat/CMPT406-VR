private var trans : Transform;
private var dir : int = 1;
var speedMult : int = 2.0;

var up_down_motion : boolean = false;
var Shock_effect: boolean = false;

var up_down_speed : float = 1f;
var up_down_multiply: float = 1f;

var JITTER : float = 5f;

function Awake () {
	trans = transform;
}

function Update () {

}

var sphereObject:Transform;
function FixedUpdate()
{
	if(sphereObject != null){
		//random speed
		var RAND_SPEEDA=speedMult;
		if(Shock_effect){
				
				RAND_SPEEDA=Random.Range(speedMult-1.1f,speedMult+JITTER); 
		}

		transform.RotateAround (sphereObject.position, Vector3.up, RAND_SPEEDA* 20 * Time.deltaTime);

		if(up_down_motion){
		
			//random speed
			var RAND_SPEED=up_down_speed;
			if(Shock_effect){
				RAND_SPEED=Random.Range(up_down_speed-0.1f,up_down_speed+JITTER/10); 
			}
			transform.position= Vector3(transform.position.x, sphereObject.transform.position.y+up_down_multiply*Mathf.Cos(Time.fixedTime+RAND_SPEED), transform.position.z);
		}
	}
}
