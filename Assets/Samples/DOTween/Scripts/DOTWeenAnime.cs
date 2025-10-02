using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DOTWeenAnime : MonoBehaviour
{
    public float shakeDuration = 1, shakeStrength = 1;
    public Text dialogue;
    public void doDiaLogue()
    {
        // dialogue.DOText("【戴夫】：\n你最好给我小心点，不给我三连，\n我就吃掉你的脑子！！！！", 4)
        //     .OnComplete(() =>
        //     {
        //         DOTween.To(() => dialogue.color, x => dialogue.color = x, Color.red, 2)
        //         .OnComplete(() =>
        //         {
        //             dialogue.transform.DOMoveX(400, 2)
        //             .OnComplete(() => dialogue.DOFade(0, 0.3f).SetLoops(-1, LoopType.Yoyo));
        //         });
        //     });

        Sequence seq = DOTween.Sequence();
        seq.Append(dialogue.DOText("【戴夫】：\n你最好给我小心点，不给我三连，\n我就吃掉你的脑子！！！！", 4))
           .Append(DOTween.To(() => dialogue.color, x => dialogue.color = x, Color.red, 2))
           .Append(dialogue.transform.DOMoveX(400, 2))
           .Append(dialogue.DOFade(0, 0.3f).SetLoops(3, LoopType.Yoyo));
    }
    public void shakeScreen()
    {
        Camera.main.transform.DOShakePosition(shakeDuration, shakeStrength);
    }
}
