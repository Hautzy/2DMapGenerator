using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
	public class ItemBehaviour: MonoBehaviour
	{
		private float _ySpeed = 0.009f;
		private float _limit = 0.16f;
		private bool _goUp = true;

	    public const float MaxLifeTime = 20;

		public Vector3 Origin { get; set; }
	    public float LifeTime { get; set; }
	    public ItemInstance Instance { get; set; }

		void Start ()
		{
			Origin = transform.position;
	    }
		
		// Update is called once per frame
		void Update ()
		{
		    LifeTime += Time.deltaTime;
		    if (LifeTime >= MaxLifeTime)
		    {
		        Destroy(gameObject);
		    }
			/* Random hover
             * if (_goUp) {
				if (transform.position.y + _ySpeed * Time.deltaTime < Origin.y + _limit) {
					transform.position += new Vector3 (0f, _ySpeed);
				} else {
					_goUp = !_goUp;
				}
			} else {
				if (transform.position.y - _ySpeed * Time.deltaTime > Origin.y - _limit) {
					transform.position -= new Vector3 (0f, _ySpeed);
				} else {
					_goUp = !_goUp;
				}
			}*/
		}
	}
}

