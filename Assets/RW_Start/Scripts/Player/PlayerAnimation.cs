
/*
 * Copyright (c) 2020 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;  // AnimatorController를 가져오기 위해 필요

namespace RW.MonumentValley
{
    public class PlayerAnimation : MonoBehaviour
    {
        private Animator animator;

        // 애니메이션 상태 이름과 해시값을 저장하는 딕셔너리
        private Dictionary<string, int> animationHashes;
        private Dictionary<string, int> parameterHashes;

        void Start()
        {
            //animator = GetComponentinChildren<Animator>();
            animator = GetComponentInChildren<Animator>();

            if (animator != null)
            {
                // 특정 레이어의 모든 애니메이션 클립을 자동으로 딕셔너리에 추가
                AddAllAnimationsToDictionary();
                AddAllParametersToDictionary();
            }
        }

        private void AddAllAnimationsToDictionary()
        {
            animationHashes = new Dictionary<string, int>();

            // Animator의 컨트롤러 가져오기
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController != null)
            {
                // Animator 레이어들을 순회
                for (int layerIndex = 0; layerIndex < animatorController.layers.Length; layerIndex++)
                {
                    AnimatorStateMachine stateMachine = animatorController.layers[layerIndex].stateMachine;

                    // 각 레이어의 상태(State)들을 순회
                    foreach (var state in stateMachine.states)
                    {
                        string animationName = state.state.name;
                        int hash = Animator.StringToHash(animationName);

                        // 애니메이션 이름과 해시값을 딕셔너리에 추가
                        if (!animationHashes.ContainsKey(animationName))
                        {
                            animationHashes.Add(animationName, hash);
                            Debug.Log($"애니메이션 추가됨: {animationName}, 해시값: {hash}");
                        }
                    }
                }
            }
        }
        private void AddAllParametersToDictionary()
        {
            parameterHashes = new Dictionary<string, int>();

            // Animator의 컨트롤러 가져오기
            AnimatorController animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController != null)
            {
                // Animator의 파라미터를 순회
                foreach (var parameter in animator.parameters)
                {
                    string parameterName = parameter.name;
                    int parameterHash = Animator.StringToHash(parameterName);

                    // 파라미터 이름과 해시값을 딕셔너리에 추가
                    if (!parameterHashes.ContainsKey(parameterName))
                    {
                        parameterHashes.Add(parameterName, parameterHash);
                        Debug.Log($"애니메이터 파라미터 추가됨: {parameterName}, 해시값: {parameterHash}");
                    }
                }
            }
        }

        // 문자열로 애니메이션을 시작하는 메서드
        public void StartAnimation(string animationName)
        {
            if (animationHashes.TryGetValue(animationName, out int hash))
            {
                
                animator?.Play(hash);  // 해시된 트리거로 애니메이션 실행
            }
            else
            {
                Debug.LogWarning($"애니메이션 '{animationName}'을 찾을 수 없습니다.");
            }
        }


        //trigger
        public void StartAnimationParameter(string parameterName)
        {
            if(parameterHashes.TryGetValue(parameterName, out int hash))
            {
                animator.SetTrigger(hash);
            }
        }
        //bool
        public void StartAnimationParameter(string parameterName, bool boolValue)
        {
            if (parameterHashes.TryGetValue(parameterName, out int hash))
            {
                animator.SetBool(hash, boolValue);
            }
        }
        //float
        public void StartAnimationParameter(string parameterName, float floatValue)
        {
            if (parameterHashes.TryGetValue(parameterName, out int hash))
            {
                animator.SetFloat(hash, floatValue);
            }
        }
        //int
        public void StartAnimationParameter(string parameterName, int intvalue)
        {
            if (parameterHashes.TryGetValue(parameterName, out int hash))
            {
                animator.SetInteger(hash, intvalue);
            }
        }

    }
}