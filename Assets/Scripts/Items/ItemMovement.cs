using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
	public class ItemMovement: MonoBehaviour
	{
		private float _ySpeed = 0.009f;
		private float _limit = 0.2f;
		private bool _goUp = true;

		private Vector3 _origin;

		void Start ()
		{
			_origin = transform.position;
	    }
		
		// Update is called once per frame
		void Update ()
		{
			if (_goUp) {
				if (transform.position.y + _ySpeed * Time.deltaTime < _origin.y + _limit) {
					transform.position += new Vector3 (0f, _ySpeed);
				} else {
					_goUp = !_goUp;
				}
			} else {
				if (transform.position.y - _ySpeed * Time.deltaTime > _origin.y - _limit) {
					transform.position -= new Vector3 (0f, _ySpeed);
				} else {
					_goUp = !_goUp;
				}
			}
		}
	}
}

