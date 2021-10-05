using UnityEngine;

namespace ChefMod {
    public class TestValueManager : MonoBehaviour {

        //how do doing attributes
        //[debugfloat("valuename", KeyCode.U, KeyCode.J, 5)] on any static value elsewhere
        //would be neat
        public static float bloob = 0.7f;

        private float tim;
        private float holdTime = 0.5f;

        //compiler flags when
        private bool testingEnabled = false;

        void Update() {
            if (!testingEnabled)
                return;

            if (!Input.GetKey(KeyCode.LeftAlt))
                return;

            manageTestValue(ref bloob, "vert", KeyCode.Keypad8, KeyCode.Keypad5, 0.05f);
        }

        private void manageTestValue(ref float value, string valueName, KeyCode upKey, KeyCode downKey, float incrementAmount) {

            if (Input.GetKeyDown(upKey)) {

                value = setTestValue(value + incrementAmount, valueName);
            }

            if (Input.GetKeyDown(downKey)) {
                
                value = setTestValue(value - incrementAmount, valueName);
            }
            

            if (Input.GetKey(upKey) || Input.GetKey(downKey)) {

                float amount = incrementAmount * (Input.GetKey(upKey) ? 1 : -1);

                tim += Time.deltaTime;

                if (tim > holdTime) {

                    value = setTestValue(value + amount, valueName);
                }
            }


            if (Input.GetKeyUp(upKey) || Input.GetKeyUp(downKey)) {
                tim = 0;
            }

        }

        private float setTestValue(float value, string print) {
            Debug.LogWarning($"{print}: {value}");
            return value;
        }
    }
}